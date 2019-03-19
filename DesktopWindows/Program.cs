using System;
using BurningKnight;
using BurningKnight.state;
using Desktop;
using Lens;

namespace DesktopWindows {
	public class Program {
		[STAThread]
		public static void Main() {
			var scale = 3;

			using (var game = new DesktopApp(new LoadState(), $"Burning Knight {Engine.Version}: I'm just setting ", Display.Width * scale, Display.Height * scale, false)) {
				game.Run();
			}
		}
	}
}