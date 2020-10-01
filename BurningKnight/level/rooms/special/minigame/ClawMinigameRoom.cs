using BurningKnight.assets.prefabs;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.special.minigame {
	public class ClawMinigameRoom : SpecialRoom {
		private const string PrefabName = "mclaw";
		private Prefab prefab;

		public ClawMinigameRoom() {
			prefab = Prefabs.Get(PrefabName);
		}
		
		public override void Paint(Level level) {
			base.Paint(level);
			
			var clip = Painter.Clip;
			
			Painter.Clip = null;
			Painter.Fill(level, this, Tile.WallA);
			Painter.Clip = clip;
			
			Painter.Prefab(level, PrefabName, Left + 1, Top + 1);
		}

		public override void PaintFloor(Level level) {
			
		}

		public override int GetMinWidth() {
			return prefab.Level.Width + 2;
		}

		public override int GetMaxWidth() {
			return prefab.Level.Width + 3;
		}

		public override int GetMinHeight() {
			return prefab.Level.Height + 2;
		}
		
		public override int GetMaxHeight() {
			return prefab.Level.Height + 3;
		}
		
		public override void SetupDoors(Level level) {
			foreach (var d in Connected.Values) {
				d.Type = DoorPlaceholder.Variant.Red;
			}
		}
	}
}