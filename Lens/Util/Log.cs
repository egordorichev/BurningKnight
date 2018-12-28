using System;
using System.Diagnostics;

namespace Lens.Util {
	public static class Log {
		public static void Info(object message) {
			Print(message, ConsoleColor.Green);
		}
		
		public static void Debug(object message) {
			Print(message, ConsoleColor.Blue);
		}
		
		public static void Error(object message) {
			Print(message, ConsoleColor.DarkRed);
		}
		
		public static void Warning(object message) {
			Print(message, ConsoleColor.DarkYellow);
		}

		private static void Print(object message, ConsoleColor color) {
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(2);

			Console.ForegroundColor = color;
			Console.WriteLine("{0:h:mm:ss} {1}():{2} {3}", DateTime.Now, frame.GetMethod().Name, frame.GetFileLineNumber(), message);
		}
	}
}