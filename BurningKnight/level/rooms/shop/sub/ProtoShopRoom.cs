using BurningKnight.level.entities.chest;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.shop.sub {
	public class ProtoShopRoom : SubShopRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, Rnd.Int(2, 4), Tiles.RandomFloor());
			
			PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect());

			var chest = new ProtoChest();
			level.Area.Add(chest);
			chest.BottomCenter = GetTileCenter() * 16 + new Vector2(8);
		}
		
		protected override bool Quad() {
			return true;
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}