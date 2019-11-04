using BurningKnight.level.entities;
using BurningKnight.level.entities.machine;

namespace BurningKnight.level.rooms.special {
	public class ChargerRoom : SpecialRoom {
		public override void Paint(Level level) {
			var charger = new Charger();
			level.Area.Add(charger);
			charger.BottomCenter = GetCenterVector();
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