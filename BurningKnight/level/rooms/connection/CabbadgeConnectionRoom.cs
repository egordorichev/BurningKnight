using BurningKnight.level.entities.plant;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.connection {
	public class CabbadgeConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			var skip = 2;
			
			Painter.Fill(level, this, 1, Tiles.RandomFloor());
			var f = Rnd.Chance() ? Tiles.RandomNewFloor() : Tile.Dirt;
			
			if (Rnd.Chance()) {
				for (var x = Left + 1; x < Right - 1; x += skip) {
					Painter.DrawLine(level, new Dot(x, Top + 1), new Dot(x, Bottom - 1), f);
					
					for (var y = Top + 1; y < Bottom; y++) {
						var p = new Plant();
						level.Area.Add(p);
						p.Variant = 255; // Cabbadge
						p.Position = new Vector2(x * 16 + 1, y * 16 - 2) + Rnd.Vector(-2, 2);
					}
				}
			} else {
				for (var y = Top + 1; y < Bottom - 1; y += skip) {
					Painter.DrawLine(level, new Dot(Left + 1, y), new Dot(Right - 1, y), f);
					
					for (var x = Left + 1; x < Right; x++) {
						var p = new Plant();
						level.Area.Add(p);
						p.Variant = 255; // Cabbadge
						p.Position = new Vector2(x * 16 + 1, y * 16 - 2) + Rnd.Vector(-2, 2);
					}
				}
			}
		}

		public override void PaintFloor(Level level) {
			
		}
		
		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 13;
		}

		public override int GetMaxHeight() {
			return 13;
		}
	}
}