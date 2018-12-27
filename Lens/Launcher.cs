using System;
using Microsoft.Xna.Framework;

namespace Lens {
	public static class Launcher {
		[STAThread]
		static void Main() {
			using (var game = new Engine("I'm just starting", Display.Width, Display.Height, false)) {
				game.Run();
			}
		}
	}
}