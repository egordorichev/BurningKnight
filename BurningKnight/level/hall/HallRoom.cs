using BurningKnight.assets.prefabs;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;

namespace BurningKnight.level.hall {
	public class HallRoom : ExitRoom {
		public const string PrefabName = "hub";
		private Prefab prefab;

		public HallRoom() {
			prefab = Prefabs.Get(PrefabName);
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Prefab(level, PrefabName, Left + 1, Top + 1);
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

		public override bool ConvertToEntity() {
			return false;
		}
	}
}