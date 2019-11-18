using BurningKnight.entity.room.controllable;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.trap {
	public class RollingSpikesRoom : TrapRoom {
		public override void Paint(Level level) {
			var a = 0;

			if (Rnd.Chance()) {
				for (var i = Left + 2; i < Right - 1; i += 2) {
					var spike = new RollingSpike();

					spike.Center = new Vector2(i, a % 2 == 0 ? Top + 2 : Bottom - 3) * 16 + new Vector2(8, 8);
					spike.StartVelocity = new Vector2(0, (a % 2 == 0 ? 1 : -1) * 32);
				
					level.Area.Add(spike);
				
					a++;
				}
			} else {
				for (var i = Top + 2; i < Bottom - 1; i += 2) {
					var spike = new RollingSpike();

					spike.Center = new Vector2(a % 2 == 0 ? Left + 2 : Right - 3, i) * 16 + new Vector2(8, 8);
					spike.StartVelocity = new Vector2((a % 2 == 0 ? 1 : -1) * 32, 0);
				
					level.Area.Add(spike);
				
					a++;
				}
			}
			
			foreach (var d in Connected.Values) {
				if (d.Type != DoorPlaceholder.Variant.Secret) {
					var w = 2;
					var h = 2;
					
					Painter.Fill(level, d.X - w, d.Y - w, w * 2 + 1, h * 2 + 1, Tile.Chasm);
				}
			}
		}
	}
}