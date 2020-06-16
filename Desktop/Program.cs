using System;
using System.Diagnostics;
using System.IO;
using Desktop.integration.crash;
using Lens.assets;
using Microsoft.Xna.Framework.Audio;

namespace Desktop {
	public class Program {
		private static void TryToRemove(string file) {
			if (File.Exists(file)) {
				try {
					File.Delete(file);
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			}
		}
		
		[STAThread]
		public static void Main() {
			CrashReporter.Bind();

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
			
			TryToRemove("burning_log.txt");
			TryToRemove("crashes.txt");

			try {
				SoundEffect.Initialize();
				Console.WriteLine("SoundEffect.Initialize() went ok");
			} catch (Exception e) {
				Assets.FailedToLoadAudio = true;
				Assets.LoadSfx = false;
				Assets.LoadMusic = false;
				
				Console.WriteLine($"Failed: {e}");
			}

			
			try {
				using (var game = new DesktopApp()) {
					game.Run();
				}
			} catch (Exception e) {
				CrashReporter.Report(e);
			}
		}
	}
}