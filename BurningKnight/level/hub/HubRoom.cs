using BurningKnight.assets.prefabs;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;

namespace BurningKnight.level.hub {
	public class HubRoom : EntranceRoom {
		private Prefab prefab;

		public HubRoom() {
			prefab = Prefabs.Get("new_hub");
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Prefab(level, "new_hub", Left + 1, Top + 1);
		}

		public override void PaintFloor(Level level) {
			
		}

		public override int GetMinWidth() {
			return prefab.Level.Width;
		}

		public override int GetMaxWidth() {
			return prefab.Level.Width + 1;
		}

		public override int GetMinHeight() {
			return prefab.Level.Height;
		}
		
		public override int GetMaxHeight() {
			return prefab.Level.Height + 1;
		}
	}
}