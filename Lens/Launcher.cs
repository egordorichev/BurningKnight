using System;
using Microsoft.Xna.Framework;

namespace Lens {
	public static class Launcher {
		[STAThread]
		static void Main() {
			using (var game = new Game()) {
				game.Run();
			}
		}
	}
}