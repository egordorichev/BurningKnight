namespace BurningKnight.level.rooms.regular {
	public class JungleRoom : RegularRoom {
		/*public override void PaintFloor(Level level) {
			// TODO: patch or smth
		}*/

		public override void Paint(Level level) {
			// TODO
		}

		public override int GetMinWidth() {
			return (10 + 2) * 3;
		}

		public override int GetMinHeight() {
			return (8 + 2) * 3;
		}

		public override int GetMaxWidth() {
			return (18 + 6) * 3;
		}

		public override int GetMaxHeight() {
			return (12 + 2) * 3;
		}
	}
}