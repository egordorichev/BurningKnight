using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmPlatformRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.BottomCenter = new Dot(Right - 2, Bottom - 2) * 16 + new Vector2(8);
		}

		public override void PlacePlayer(Room room, Player player) {
			player.BottomCenter = new Dot(Left + 2, Top + 2) * 16 + new Vector2(8);
		}

		public override void Paint(Level level, Room room) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			
			Painter.Fill(level, new Rect().Setup(Left + 1, Top + 1, 2, 2), Tiles.RandomFloor());
			Painter.Fill(level, new Rect().Setup(Left + 1, Top + GetHeight() / 2, 2, 2), Tiles.RandomFloor());
			Painter.Fill(level, new Rect().Setup(Left + 1, Bottom - 2, 2, 2), Tiles.RandomFloor());
			
			Painter.Fill(level, new Rect().Setup(Right - 2, Top + 1, 2, 2), Tiles.RandomFloor());
			Painter.Fill(level, new Rect().Setup(Right - 2, Top + GetHeight() / 2, 2, 2), Tiles.RandomFloor());
			
			Painter.Fill(level, new Rect().Setup(Right - 3, Bottom - 3, 3, 3), Tiles.RandomFloor());
			Painter.Fill(level, new Rect().Setup(Right - 3, Bottom - 3, 2, 2), Tile.WallA);
			Painter.Fill(level, new Rect().Setup(Right - 2, Bottom - 2, 2, 2), Tiles.RandomFloor());
			
			Platform(level, Left + 3, Top + 1);
			Platform(level, Right - 4, Top + GetHeight() / 2);
			Platform(level, Left + 3, Bottom - 2);
			
			Platform(level, Right - 2, Top + GetHeight() / 2 - 3, true);
			Platform(level, Left + 1, Top + GetHeight() / 2 + 5, true);
		}

		private void Platform(Level level, int x, int y, bool vertical = false) {
			level.Area.Add(new MovingPlatform {
				Controller = vertical ? PlatformController.UpDown : PlatformController.LeftRight,
				Position = new Vector2(x, y) * 16
			});
		}

		public override int GetMinWidth() {
			return 16;
		}

		public override int GetMinHeight() {
			return 20;
		}

		public override int GetMaxWidth() {
			return 17;
		}

		public override int GetMaxHeight() {
			return 21;
		}
	}
}