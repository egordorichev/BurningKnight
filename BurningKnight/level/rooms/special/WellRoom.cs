using BurningKnight.level.entities;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class WellRoom : SpecialRoom {
		public override void Paint(Level level) {
			var well = new Well();
			level.Area.Add(well);
			well.Center = GetCenter() * 16 + new Vector2(8);
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}
	}
}