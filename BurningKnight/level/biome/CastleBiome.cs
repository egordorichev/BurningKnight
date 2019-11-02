using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class CastleBiome : Biome {
		public CastleBiome() : base("Born to do rogueries", Biome.Castle, "castle_biome", new Color(14, 7, 27)) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Modifiers.Add((l, x, y) => {
				var f = Run.Depth == 1;
				
				var r = (byte) (f ? Tiles.RandomFloor() : Tile.Chasm);
				
				if (l.Get(x, y, true) == Tile.Lava || (f && l.Get(x, y) == Tile.Chasm)) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = r;
				}
			});
		}
	}
}