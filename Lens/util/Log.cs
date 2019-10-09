using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Lens.util {
	public static class Log {
		public static bool WriteToFile = false;
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

			File.WriteAllText("log.txt", builder.ToString());
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
			var prev = stackTrace.GetFrame(3);
			var text = $" {DateTime.Now:h:mm:ss} {Path.GetFileName(frame.GetFileName())}:{frame.GetMethod().Name}():{frame.GetFileLineNumber()} <= {Path.GetFileName(prev.GetFileName())}:{prev.GetMethod().Name}():{prev.GetFileLineNumber()}";

			builder?.Append(text);
			builder?.AppendLine(message == null ? "null" : message.ToString());
			
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(text);
		}
	}
}