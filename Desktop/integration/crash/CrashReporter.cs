using System;
using System.IO;
using System.Reflection;
using System.Text;
using Lens;
using Lens.util;

namespace Desktop.integration.crash {
	public class CrashReporter {
		public static void Bind() {
			
			var domain = AppDomain.CurrentDomain;
			domain.UnhandledException += ExceptionHandler;
		}

		private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
			var e = (Exception) args.ExceptionObject;
			var builder = new StringBuilder();
			
			builder.AppendLine("--- Burning Knight has crashed :( ---");
			builder.AppendLine("PLEASE REPORT THIS TO REXCELLENT GAMES");
			builder.AppendLine("via contact@rexcellentgames.com or @egordorichev on twitter");
		
			builder.AppendLine("--- Info --- ");

			builder.AppendLine($"Date: {DateTime.Now:dd.MM.yyyy h:mm tt}");
			builder.AppendLine($"OS: {Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? 64 : 32 )} bit");

			builder.AppendLine($"Mono version: {GetMonoVersion()}");
			builder.AppendLine($"Burning Knight version: {Engine.Version}");
			
			builder.AppendLine("--- Error --- ");
			
			builder.AppendLine(e.Message);
			builder.AppendLine();
			builder.AppendLine(e.StackTrace);

			builder.AppendLine("--- Please report this to help us fix the issue! <3 ---");

			var message = builder.ToString();
			Log.Error(message);
			
			File.AppendAllText("crashes.txt", message);
		}

		private static string GetMonoVersion() {
			var type = Type.GetType("Mono.Runtime");

			if (type != null) {
				var method = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);

				if (method != null) {
					return (string) method.Invoke(null, null);
				}
			}

			return "Unknown";
		}
	}
}