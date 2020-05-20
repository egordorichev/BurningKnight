using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmBulletDodgeRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.BottomCenter = new Vector2((Right - 1) * 16 - 8, GetTileCenter().Y * 16 + 8);
		}

		public override void PlacePlayer(Room room, Player player) {
			player.BottomCenter = new Vector2((Left + 1) * 16 + 8, GetTileCenter().Y * 16 + 8);
		}

		public override void Paint(Level level, Room room) {
			var x = (Right - 2) * 16 - 8;
			var wall = Tiles.Pick(Tile.WallA, Tile.WallB, Tile.EvilWall);
			
			Painter.DrawLine(level, new Dot(Left + 1, Top + 1), new Dot(Right - 7, Top + 1), wall);
			Painter.DrawLine(level, new Dot(Left + 1, Bottom - 1), new Dot(Right - 7, Bottom - 1), wall);
			
			for (var y = Top + 2; y < Bottom - 1; y++) {
				var turret = new Turret {
					StartingAngle = 4
				};

				level.Area.Add(turret);
				turret.BottomCenter = new Vector2(x, y * 16 + 12);
			}
		}

		public override int GetMinWidth() {
			return 30;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxWidth() {
			return 40;
		}

		public override int GetMaxHeight() {
			return 13;
		}
	}
}