using System;
using BurningKnight.state;
using Lens;

namespace BurningKnight {
	public class Program {
		[STAThread]
		public static void Main() {
			var scale = 2;

			using (var game = new BK(new InGameState(), $"Burning Knight {Engine.Version}: Burn, baby, burn!", Display.Width * scale, Display.Height * scale, false)) {
				game.Run();
			}
		}
	}
}