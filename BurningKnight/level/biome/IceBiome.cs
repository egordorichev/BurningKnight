using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class IceBiome : Biome {
		public IceBiome() : base("Frozen to the bones", Biome.Ice, "ice_biome", new Color(146, 161, 185)) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);

			painter.Grass = 0;
			painter.Water = 0.5f;
			painter.Dirt = 0;
			painter.Cobweb = 0;
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Water) {
					l.Set(x, y, Tile.Ice);
				}
			});
		}
	}
}