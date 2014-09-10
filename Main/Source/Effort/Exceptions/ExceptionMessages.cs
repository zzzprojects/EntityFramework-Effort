// --------------------------------------------------------------------------------------------
// <copyright file="ExceptionMessages.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Exceptions
{
    using System;
    using Effort.Provider;

    internal class ExceptionMessages
    {
        private static readonly string InvariantName = 
            EffortProviderConfiguration.ProviderInvariantName;

        private static readonly string FactoryType =
            typeof(EffortProviderFactory).FullName +
            ", " +
            typeof(EffortProviderFactory).Assembly.GetName().Name;

        private static readonly string ProviderServicesType =
            typeof(EffortProviderServices).FullName +
            ", " +
            typeof(EffortProviderServices).Assembly.GetName().Name;

        private static readonly string Break = Environment.NewLine;

        private static readonly string AutomaticRegistationFailedResolveCode =
             "a) Call the Effort.Provider.EffortProviderConfiguration.RegisterProvider() " +
            "method at entry point of the application";

        private static readonly string AutomaticRegistationFailedResolveConfig =
            "b) Add the following configuration to the App.config file:" +
                                                                                  Break +
            "   <system.data>"                                                  + Break +
            "      <DbProviderFactories>"                                       + Break +
            "         <add name=\"" + InvariantName + "\""                      + Break +
            "              invariant=\"" + InvariantName + "\""                 + Break +
            "              description=\"" + InvariantName + "\""               + Break +
            "              type=\"" + FactoryType + "\" />"                     + Break +
            "      </DbProviderFactories>"                                      + Break +
            "   </system.data>" +
#if !EFOLD
            Break + Break + Break +
            "   <entityFramework>"                                              + Break +
            "      <providers>"                                                 + Break +
            "         <provider invariantName=\"" + InvariantName + "\""        + Break +
            "                   type=\"" + ProviderServicesType + "\" />"       + Break +
            "      </providers>"                                                + Break +
            "   </entityFramework>" +
#endif
            "";

        public static readonly string AutomaticRegistrationFailed =
            "The Effort library failed to register its provider automatically, so manual " +
            "registration is required." +
            Break + Break +
            AutomaticRegistationFailedResolveCode +
            Break + Break +
            "or" +
            Break + Break +
            AutomaticRegistationFailedResolveConfig;

        public static readonly string EntityPropertyAssignFailed = 
             "An unhandled exception occurred while trying to assign value '{0}' to Property" +
             "'{1}' of type '{2}' during entity initialization for table '{3}'";

        public static readonly string TableInitializationFailed =
            "Unhandled exception while trying to initialize the content of '{0}' table";

        public static readonly string DbExpressionTransformationNotImplemented =
            "Transformation of {0} expression is not implemented";

        public static readonly string TableNotFound =
            "Table '{0}' was not found. The database was probably not initialized." +
            Break + Break +
            "If using CodeFirst try to add the following line:" +
            Break +
            "context.Database.CreateIfNotExists()";

        public static readonly string DatabaseNotInitialized =
            "Database has not been initialized." + 
            Break + Break +
             "If using CodeFirst try to add the following line:" +
            Break +
            "context.Database.CreateIfNotExists()";
    }
}
