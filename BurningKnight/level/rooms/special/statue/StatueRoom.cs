using BurningKnight.level.entities.statue;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.statue {
	public class StatueRoom : SpecialRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, Rnd.Int(2, 4), Tiles.RandomFloor());
			
			PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect());

			var statue = GetStatue();
			level.Area.Add(statue);
			statue.BottomCenter = GetTileCenter() * 16 + new Vector2(8);
		}

		protected virtual Statue GetStatue() {
			return new DiceStatue();
		}

		protected override bool Quad() {
			return true;
		}

		public override int GetMinWidth() {
			return 13;
		}

		public override int GetMinHeight() {
			return 13;
		}

		public override int GetMaxWidth() {
			return 16;
		}

		public override int GetMaxHeight() {
			return 16;
		}
	}
}