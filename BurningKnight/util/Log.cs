using System;

namespace BurningKnight.util {
	public class Log {
		public static void Error(Object String) {
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(String);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void Info(Object String) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(String);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void Debug(Object String) {
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(String);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}