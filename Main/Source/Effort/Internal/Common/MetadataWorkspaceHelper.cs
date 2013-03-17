// --------------------------------------------------------------------------------------------
// <copyright file="MetadataWorkspaceHelper.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.Common
{
    using System;
    using System.Collections.Generic; 
#if !EFOLD
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Common;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
#endif
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;
    using Effort.Internal.StorageSchema;

    internal static class MetadataWorkspaceHelper
    {
        //// Code from:
        //// http://bit.ly/TX9EMX

        private static byte[] systemPublicKeyToken = { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A };
        private static Regex resRegex = new Regex(@"^res://(?<assembly>.*)/(?<resource>.*)$");
        private static string httpContextTypeName = "System.Web.HttpContext, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        public static EntityContainer GetEntityContainer(MetadataWorkspace workspace)
        {
            // Get the Storage Schema Definition
            ItemCollection ssdl = workspace.GetItemCollection(DataSpace.SSpace);

            EntityContainer entityContainer = ssdl.OfType<EntityContainer>().FirstOrDefault();

            if (entityContainer == null)
            {
                // Invalid SSDL
                throw new InvalidOperationException("The Storage Schema Definition does not contain any EntityContainer");
            }

            return entityContainer;
        }

        public static MetadataWorkspace Rewrite(string metadata, string providerInvariantName, string providerManifestToken)
        {
            DbProviderManifest providerManifest = ProviderHelper.GetProviderManifest(providerInvariantName, providerManifestToken);
            
            var csdl = new List<XElement>();
            var ssdl = new List<XElement>();
            var msl = new List<XElement>();

            ParseMetadata(metadata, csdl, ssdl, msl);

            foreach (var ssdlFile in ssdl)
            {
                UniversalStorageSchemaModifier.Instance.Modify(ssdlFile, new ProviderInformation(providerInvariantName, providerManifestToken));
            }

            MetadataWorkspace workspace = CreateMetadataWorkspace(csdl, ssdl, msl);

            return workspace;
        }

        public static MetadataWorkspace CreateMetadataWorkspace(List<XElement> csdl, List<XElement> ssdl, List<XElement> msl)
        {
            EdmItemCollection eic = new EdmItemCollection(csdl.Select(c => c.CreateReader()));
            StoreItemCollection sic = new StoreItemCollection(ssdl.Select(c => c.CreateReader()));
            StorageMappingItemCollection smic = new StorageMappingItemCollection(eic, sic, msl.Select(c => c.CreateReader()));

            // and create metadata workspace based on them.
#if !EFOLD
            MetadataWorkspace workspace =
                new MetadataWorkspace(
                    () => eic,
                    () => sic,
                    () => smic);
#else
            // Obsolete API
            MetadataWorkspace workspace = new MetadataWorkspace();
            workspace.RegisterItemCollection(eic);
            workspace.RegisterItemCollection(sic);
            workspace.RegisterItemCollection(smic);
#endif
            return workspace;
        }

        public static void ParseMetadata(string metadata, List<XElement> csdl, List<XElement> ssdl, List<XElement> msl)
        {
            foreach (string component in metadata.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()))
            {
                string translatedComponent = component;

                if (translatedComponent.StartsWith("~", StringComparison.Ordinal))
                {
                    var httpContextType = Type.GetType(httpContextTypeName, true);
                    dynamic context = (dynamic)httpContextType.GetProperty("Current").GetValue(null, null);
                    if (context == null)
                    {
                        throw new NotSupportedException("Paths prefixed with '~' are not supported outside of ASP.NET.");
                    }

                    translatedComponent = context.Server.MapPath(translatedComponent);
                }

                if (translatedComponent.StartsWith("res://", StringComparison.Ordinal))
                {
                    ParseResources(translatedComponent, csdl, ssdl, msl);
                }
                else if (Directory.Exists(translatedComponent))
                {
                    ParseDirectory(translatedComponent, csdl, ssdl, msl);
                }
                else if (translatedComponent.EndsWith(".csdl", StringComparison.OrdinalIgnoreCase))
                {
                    csdl.Add(XElement.Load(translatedComponent));
                }
                else if (translatedComponent.EndsWith(".ssdl", StringComparison.OrdinalIgnoreCase))
                {
                    ssdl.Add(XElement.Load(translatedComponent));
                }
                else if (translatedComponent.EndsWith(".msl", StringComparison.OrdinalIgnoreCase))
                {
                    msl.Add(XElement.Load(translatedComponent));
                }
                else
                {
                    throw new NotSupportedException("Unknown metadata component: " + component);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to ignore exceptions during loading of references.")]
        private static void ParseResources(string resPath, List<XElement> csdl, List<XElement> ssdl, List<XElement> msl)
        {
            Match match = resRegex.Match(resPath);
            if (!match.Success)
            {
                throw new NotSupportedException("Not supported resource path: " + resPath);
            }

            string assemblyName = match.Groups["assembly"].Value;
            string resourceName = match.Groups["resource"].Value;

            List<Assembly> assembliesToConsider = new List<Assembly>();
            if (assemblyName == "*")
            {
                assembliesToConsider.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            }
            else
            {
                assembliesToConsider.Add(Assembly.Load(new AssemblyName(assemblyName)));
            }

            var domainManager = AppDomain.CurrentDomain.DomainManager;
            if (domainManager != null && domainManager.EntryAssembly != null)
            {
                foreach (AssemblyName asmName in domainManager.EntryAssembly.GetReferencedAssemblies())
                {
                    try
                    {
                        var asm = Assembly.Load(asmName);
                        if (!assembliesToConsider.Contains(asm))
                        {
                            assembliesToConsider.Add(asm);
                        }
                    }
                    catch
                    {
                        // ignore errors
                    }
                }
            }

            foreach (Assembly asm in assembliesToConsider.Where(asm => 
                !IsEcmaAssembly(asm) && 
                !IsSystemAssembly(asm) &&
                !asm.IsDynamic))
            {
                using (Stream stream = asm.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        continue;
                    }

                    if (resourceName.EndsWith(".csdl", StringComparison.OrdinalIgnoreCase))
                    {
                        csdl.Add(XElement.Load(XmlReader.Create(stream)));
                        return;
                    }
                    else if (resourceName.EndsWith(".ssdl", StringComparison.OrdinalIgnoreCase))
                    {
                        ssdl.Add(XElement.Load(XmlReader.Create(stream)));
                        return;
                    }
                    else if (resourceName.EndsWith(".msl", StringComparison.OrdinalIgnoreCase))
                    {
                        msl.Add(XElement.Load(XmlReader.Create(stream)));
                        return;
                    }
                }
            }

            throw new InvalidOperationException("Resource " + resPath + " not found.");
        }

        private static bool IsEcmaAssembly(Assembly asm)
        {
            byte[] publicKey = asm.GetName().GetPublicKey();

            // ECMA key is special, as it is only 4 bytes long
            if (publicKey != null && publicKey.Length == 16 && publicKey[8] == 0x4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsSystemAssembly(Assembly asm)
        {
            byte[] publicKeyToken = asm.GetName().GetPublicKeyToken();

            if (publicKeyToken != null && publicKeyToken.Length == systemPublicKeyToken.Length)
            {
                for (int i = 0; i < systemPublicKeyToken.Length; ++i)
                {
                    if (systemPublicKeyToken[i] != publicKeyToken[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ParseDirectory(string directory, List<XElement> csdl, List<XElement> ssdl, List<XElement> msl)
        {
            foreach (string file in Directory.GetFiles(directory, "*.csdl"))
            {
                csdl.Add(XElement.Load(file));
            }

            foreach (string file in Directory.GetFiles(directory, "*.ssdl"))
            {
                ssdl.Add(XElement.Load(file));
            }

            foreach (string file in Directory.GetFiles(directory, "*.msl"))
            {
                msl.Add(XElement.Load(file));
            }
        }
    }
}
