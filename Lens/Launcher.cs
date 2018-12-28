using System;
using Microsoft.Xna.Framework;

namespace Lens {
	public static class Launcher {
		[STAThread]
		public static void Main() {
			using (var game = new Engine(null, "I'm just starting", Display.Width, Display.Height, false)) {
				game.Run();
			}
		}
	}
}