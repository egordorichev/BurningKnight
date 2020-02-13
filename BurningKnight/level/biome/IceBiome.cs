using BurningKnight.assets.lighting;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class IceBiome : Biome {
		public IceBiome() : base("Frozen to the bones", Biome.Ice, "ice_biome", new Color(146, 161, 185)) {
			
		}

		public override void Apply() {
			base.Apply();

			Lights.ClearColor = new Color(0.2f, 0.45f, 1f, 1f);
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);

			painter.Grass = 0;
			painter.Water = 0.45f;
			painter.Dirt = 0;
			painter.Cobweb = 0;
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Water) {
					l.Set(x, y, Tile.Ice);
				}
			});
			
			painter.Modifiers.Add((l, rm, x, y) => {
				var r = (byte) (Tiles.RandomFloor());
				
				if (l.Get(x, y, true) == Tile.Lava) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = r;
				}
			});
		}

		public override bool HasPaintings() {
			return false;
		}
	}
}