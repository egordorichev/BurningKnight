using System;
using System.Diagnostics;

namespace Desktop {
	public class Program {
		[STAThread]
		public static void Main() {
			if (!Environment.Is64BitOperatingSystem) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Burning Knight can't run on 32 bit OS, sorry :(");
				Console.ForegroundColor = ConsoleColor.White;

				return;
			}

			var current = Process.GetCurrentProcess();
			var processes = Process.GetProcessesByName(current.ProcessName);

			Console.WriteLine($"Found a total of {processes.Length} processes with name {current.ProcessName}");

			foreach (var process in processes) {
				if (process.Id != current.Id) {
					try {
						process.CloseMainWindow();
						process.Kill();
						Console.WriteLine("Killing older process");
					} catch (Exception e) {
						Console.WriteLine($"Failed: {e}");
					}
				}
			}

			using (var game = new DesktopApp()) {
				game.Run();
			}
		}
	}
}