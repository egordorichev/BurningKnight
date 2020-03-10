using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class LibraryBiome : Biome {
		public LibraryBiome() : base("Hidden knowledge", Biome.Library, "library_biome", new Color(28, 18, 28)) {
		}

		public override void ModifyPainter(Level level, Painter painter) {
			base.ModifyPainter(level, painter);

			painter.Grass = 0;
			painter.Water = 0;
			painter.Dirt = 0;
			painter.Cobweb = 0.3f;
		}
	}
}