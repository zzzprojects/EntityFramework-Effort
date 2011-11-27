using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;

namespace Effort.CodeFirst
{
	public static class CodeFirstEmulatorProviderConfiguration
	{
		public static readonly string ProviderInvariantName = "EffortCodeFirstEmulatorProvider";

		private static volatile bool registered = false;
		private static object synch = new object();

		public static void RegisterProvider()
		{
			if (!registered)
			{
				lock (synch)
				{
					if (!registered)
					{
						DbProviderFactoryBase.RegisterProvider("Effort Code First Emulator Provider", ProviderInvariantName,
							typeof(CodeFirstEmulatorProviderFactory));
						registered = true;
					}
				}
			}
		}
	}
}
