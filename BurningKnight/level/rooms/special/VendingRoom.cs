using BurningKnight.level.entities.machine;

namespace BurningKnight.level.rooms.special {
	public class VendingRoom : SpecialRoom {
		public override void Paint(Level level) {
			var machine = new VendingMachine();
			level.Area.Add(machine);
			machine.BottomCenter = GetCenterVector();
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