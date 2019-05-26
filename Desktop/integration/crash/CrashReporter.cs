using System;
using System.Reflection;
using Lens;

namespace Desktop.integration.crash {
	public class CrashReporter {
		public static void Bind() {
			
			var domain = AppDomain.CurrentDomain;
			domain.UnhandledException += ExceptionHandler;
		}

		private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
			var e = (Exception) args.ExceptionObject;
			
			Console.ForegroundColor = ConsoleColor.Red;
			
			Console.WriteLine("--- Burning Knight has crashed :( ---");
			Console.WriteLine("PLEASE REPORT THIS TO REXCELLENT GAMES");
			Console.WriteLine("via contact@rexcellentgames.com or @egordorichev on twitter");
		
			Console.WriteLine("--- Info --- ");

			Console.WriteLine($"Date: {DateTime.Now:dd.MM.yyyy h:mm tt}");
			Console.WriteLine($"OS: {Environment.OSVersion}");

			Console.WriteLine($"Mono version: {GetMonoVersion()}");
			Console.WriteLine($"Burning Knight version: {Engine.Version}");
			
			Console.WriteLine("--- Error --- ");
			
			Console.WriteLine(e.Message);
			Console.WriteLine();
			Console.WriteLine(e.StackTrace);

			Console.WriteLine("--- Please report this to help us fix the issue! <3 ---");
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