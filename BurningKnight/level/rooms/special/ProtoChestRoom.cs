using BurningKnight.level.entities.chest;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class ProtoChestRoom : SpecialRoom {
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
	}
}