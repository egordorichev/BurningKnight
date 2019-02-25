using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.levels.library {
	public class LibraryLevel : RegularLevel {
		public LibraryLevel() {
			Terrain.LoadTextures(2);
			this.Uid = 2;
		}

		public override string GetName() {
			return Locale.Get("ancient_library");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Hidden knowledge";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0.35f).SetWater(0);
		}

		protected override int GetNumConnectionRooms() {
			return 0;
		}
	}
}