using BurningKnight.level.entities;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class WellRoom : SpecialRoom {
		public override void Paint(Level level) {
			var well = new Well();
			level.Area.Add(well);
			well.Center = GetTileCenter() * 16 + new Vector2(8);
		}

		public override int GetMinWidth() {
			return 6;
		}

		public override int GetMinHeight() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}
	}
}