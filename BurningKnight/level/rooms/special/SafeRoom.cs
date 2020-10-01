using BurningKnight.level.entities;

namespace BurningKnight.level.rooms.special {
	public class SafeRoom : SpecialRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			
			var safe = new Safe();
			level.Area.Add(safe);
			safe.BottomCenter = GetCenterVector();
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