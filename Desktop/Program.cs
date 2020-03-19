using System;
using System.Diagnostics;
using Aseprite;
using Lens.util.file;

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
			
			using (var game = new DesktopApp()) {
				game.Run();
			}
		}
	}
}