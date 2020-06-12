using System.Collections.Generic;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.util.math;
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
			
			painter.Modifiers.Add((l, rm, x, y) => {
				var r = (byte) (Tiles.RandomFloor());
				var t = l.Get(x, y, true);
				
				if (t == Tile.Lava) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = r;
				} else if (t.IsRock() || t == Tile.MetalBlock) {
					var i = l.ToIndex(x, y);

					l.Liquid[i] = 0;
					l.Tiles[i] = (byte) Tile.Chasm;
				}
			});
		}
				

		public override int GetNumRegularRooms() {
			return (int) (base.GetNumRegularRooms() * 1.4f);
		}

		public override Builder GetBuilder() {
			var builder = new LoopBuilder().SetShape(2,
				Rnd.Float(0.4f, 0.7f),
				Rnd.Float(0f, 0.5f));

			return builder;
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorA || tile == Tile.FloorC) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}
		
		private static Color mapColor = new Color(138, 72, 54);

		public override Color GetMapColor() {
			return mapColor;
		}
	}
}