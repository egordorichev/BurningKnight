using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.level.hub {
	public class HubExitRoom : ExitRoom {
		public override void Paint(Level level) {
			Place(level, GetCenter());
		}

		protected override void Fill(Level level) {
			
		}
	}
}