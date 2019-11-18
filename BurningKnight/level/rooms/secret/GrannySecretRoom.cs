namespace BurningKnight.level.rooms.secret {
	public class GrannySecretRoom : SecretRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Prefab(level, "granny", Left, Top);
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 9;
		}

		public override int GetMaxHeight() {
			return 6;
		}
	}
}