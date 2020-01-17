using BurningKnight.entity.door;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class DarkMarketEntranceRoom : SpecialRoom {
		public override void Paint(Level level) {
			var hw = (int) ((GetWidth() - 1) / 2f);
			
			var entrance = new HiddenEntrance();
			level.Area.Add(entrance);
			entrance.Position = new Vector2(Left + hw, Top + 2) * 16;

			if (Rnd.Chance()) {
				Painter.Fill(level, new Rect(Left + 1, Top + 1).Resize(GetWidth() - 2, 3), Tiles.Pick(Tile.WallA, Tile.Chasm, Tile.Planks, Tile.WallB));
			}
			
			Painter.Fill(level, new Rect(Left + hw - 1, Top + 1).Resize(3, 3), Rnd.Chance(10) ? Tile.FloorD : Tiles.RandomFloor());
			Painter.DrawLine(level, new Dot(Left + 1, Top + 4), new Dot(Right - 1, Top + 4), Tile.WallA);

			var d = new Dot(Left + hw, Top + 4);
			Painter.Set(level, d, Tiles.RandomFloor());

			var door = new HeadDoor();
			
			level.Area.Add(door);

			var offset = door.GetOffset();

			door.CenterX = d.X * 16 + 8 + offset.X;
			door.Bottom = d.Y * 16 + 17.01f + offset.Y - 8; // .1f so that it's depth sorted to the front of the wall
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right || P.Y == Top) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}
		
		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 15;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 12;
		}
	}
}