using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Lens.Util {
	public static class Log {
		public static bool WriteToFile = true;
		private static StringBuilder builder;

		public static void Open() {
			if (WriteToFile) {
				builder = new StringBuilder();
			}
		}

		public static void Close() {
			if (builder == null) {
				return;
			}

			System.IO.File.AppendAllText("log.txt", builder.ToString());
			builder = null;
		}
		
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
			var stackTrace = new StackTrace(true);
			var frame = stackTrace.GetFrame(2);
			var text = String.Format("{0:h:mm:ss} {1} {2}():{3} {4}", DateTime.Now, Path.GetFileName(frame.GetFileName()), frame.GetMethod().Name,
				frame.GetFileLineNumber(), message);

			builder?.AppendLine(text);
			
			Console.ForegroundColor = color;
			Console.WriteLine(text);
		}
	}
}