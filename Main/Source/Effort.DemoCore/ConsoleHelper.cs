using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Demo
{
	public static class ConsoleHelper
	{
		public static void Init()
		{
			try
			{
				Console.SetBufferSize(150, 3000);
				Console.SetWindowSize(150, 60);
			}
			catch
			{
			}
		}
		public static void Print(string text)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(text);
			Console.ForegroundColor = oldColor;
			Console.ReadLine();
		}
	}
}
