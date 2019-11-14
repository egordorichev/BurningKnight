using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ImGuiNET;

namespace Lens.util {
	public static class Log {
		public static bool WriteToFile = !Engine.Debug;
		private static StringBuilder builder;
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);

		public static void Open() {
			builder = new StringBuilder();
		}

		public static void Close() {
			if (!WriteToFile) {
				return;
			}

			File.WriteAllText("burning_log.txt", builder.ToString());
		}
		
		public static void Info(object message) {
			Print(message, ConsoleColor.Green, "INF");
		}
		
		public static void Debug(object message) {
			Print(message, ConsoleColor.Blue, "DBG");
		}
		
		public static void Error(object message) {
			Print(message, ConsoleColor.DarkRed, "ERR");
		}
		
		public static void Warning(object message) {
			Print(message, ConsoleColor.DarkYellow, "WRN");
		}

		private static void Print(object message, ConsoleColor color, string type) {
			var stackTrace = new StackTrace(true);
			var frame = stackTrace.GetFrame(2);
			var prev = stackTrace.GetFrame(3);

			#if DEBUG
				var text = $"{DateTime.Now:h:mm:ss} {Path.GetFileName(frame.GetFileName())}:{frame.GetMethod().Name}():{frame.GetFileLineNumber()} <= {Path.GetFileName(prev.GetFileName())}:{prev.GetMethod().Name}():{prev.GetFileLineNumber()} ";
			#else 
				var text = $"{DateTime.Now:h:mm:ss} {frame.GetMethod().Name}() <= {prev.GetMethod().Name}() ";
			#endif

			builder?.Append($"{type} {text}");
			builder?.AppendLine(message == null ? "null" : message.ToString());
			
			Console.ForegroundColor = color;
			Console.Write($"{message} ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(text);
		}

		public static void RenderDebug() {
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Log")) {
				ImGui.End();
				return;
			}

			if (ImGui.Button("Clear")) {
				builder.Clear();
			}
			
			ImGui.Separator();
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			ImGui.Text(builder.ToString());
			ImGui.EndChild();
			ImGui.End();
		}
	}
}