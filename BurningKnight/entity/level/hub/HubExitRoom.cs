using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.level.hub {
	public class HubExitRoom : ExitRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Prefab(level, "exit", Left, Top);
			Place(level, GetCenter());
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