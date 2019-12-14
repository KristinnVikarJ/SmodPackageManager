using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageManagerClient
{
	public static class Logger
	{
		private static readonly StreamWriter _logger;

		static Logger()
		{
			_logger = new StreamWriter("log.txt", true);
		}
		public static void SilentWriteLine(object line) => _logger.WriteLine(line);
		public static void SilentWriteLine(string line) => _logger.WriteLine(line);
		public static void WriteLine(object line, ConsoleColor color = ConsoleColor.White) => WriteLine(line.ToString(), color);
		public static void WriteLine(string line, ConsoleColor color = ConsoleColor.White) => Write(line + Environment.NewLine, color);
		public static void Write(object text, ConsoleColor color = ConsoleColor.White) => Write(text.ToString(), color);
		public static void Write(string text, ConsoleColor color = ConsoleColor.White)
		{
			ConsoleColor prevColor = Console.ForegroundColor;

			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = prevColor;

			_logger.Write(text);
		}
	}

}
