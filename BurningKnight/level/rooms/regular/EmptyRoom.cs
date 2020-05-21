namespace BurningKnight.level.rooms.regular {
	public class EmptyRoom : RegularRoom {
		public override void Paint(Level level) {
			
		}

		public override bool ShouldSpawnMobs() {
			return false;
		}
	}
}