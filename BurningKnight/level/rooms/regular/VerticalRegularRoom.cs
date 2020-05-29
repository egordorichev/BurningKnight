namespace BurningKnight.level.rooms.regular {
	public class VerticalRegularRoom : RegularRoom {
		public override int GetMinHeight() {
			return SmallerRooms() ? 10 : 12;
		}

		public override int GetMinWidth() {
			return SmallerRooms() ? 8 : 10;
		}

		public override int GetMaxHeight() {
			return SmallerRooms() ? 16 : 24;
		}

		public override int GetMaxWidth() {
			return SmallerRooms() ? 12 : 14;
		}
	}
}