using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.level.hub {
	public class HubEntranceRoom : EntranceRoom {
		public HubEntranceRoom() {
			IgnoreEntranceRooms = true;
		}

		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Prefab(level, "entrance", Left, Top);
		}
		
		protected override void Fill(Level level) {
			
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}
	}
}