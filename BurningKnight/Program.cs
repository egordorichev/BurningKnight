using System;
using BurningKnight.state;
using Lens;

namespace BurningKnight {
	public class Program {
		[STAThread]
		public static void Main() {
			var scale = 2;

			using (var game = new BK(new LoadState(), $"Burning Knight {Engine.Version}: I'm just setting things up", Display.Width * scale, Display.Height * scale, false)) {
				game.Run();
			}
		}
	}
}