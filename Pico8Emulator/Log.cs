using System;

namespace Pico8Emulator {
	public static class Log {
		public static void Info(object s) {
			Console.WriteLine(s);
		}

		public static void Error(object s) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(s);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}