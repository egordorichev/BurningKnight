using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class IceBiome : Biome {
		public IceBiome() : base("Frozen to the bones", Biome.Ice, "ice_biome", new Color(146, 161, 185)) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Modifiers.Add((l, x, y) => {
				if (l.Get(x, y, true) == Tile.Water) {
					l.Set(x, y, Tile.Ice);
				}
			});
		}
	}
}