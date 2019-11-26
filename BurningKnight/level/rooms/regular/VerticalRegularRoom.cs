namespace BurningKnight.level.rooms.regular {
	public class VerticalRegularRoom : RegularRoom {
		public override int GetMinHeight() {
			return 10 + 2;
		}

		public override int GetMinWidth() {
			return 8 + 2;
		}

		public override int GetMaxHeight() {
			return 18 + 6;
		}

		public override int GetMaxWidth() {
			return 12 + 2;
		}
	}
}