using BurningKnight.assets.lighting;
using BurningKnight.level.tile;
using Lens.util.math;
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
			painter.Dirt = 0.45f;
			painter.Cobweb = 0;

			painter.DirtTile = Tile.Snow;
			
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
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Dirt) {
					l.Set(x, y, Tile.Snow);
				}
			});
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorA || tile == Tile.FloorC) {
				return $"player_step_snow_{Rnd.Int(1, 4)}";
			} else if (tile == Tile.FloorB) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}

		public override bool HasPaintings() {
			return false;
		}
	}
}