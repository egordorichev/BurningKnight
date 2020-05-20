using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmEnemyRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.Center = GetTileCenter() * 16 - new Vector2(0, 64);
		}

		public override void PlacePlayer(Room room, Player player) {
			player.Center = GetTileCenter() * 16;
		}

		public override void Paint(Level level, Room room) {
			var c = GetCenter();
			
			Painter.Fill(level, new Rect().Setup(c.X - 2, c.Y - 2, 5, 5), Tile.WallA);
			Painter.Fill(level, new Rect().Setup(c.X - 1, c.Y - 1, 3, 3), Tiles.RandomFloor());

			var placed = false;

			if (Rnd.Chance()) {
				placed = true;
				Painter.Set(level, new Dot(c.X, c.Y - 2), Tiles.RandomFloor());
			}
			
			if (Rnd.Chance()) {
				placed = true;
				Painter.Set(level, new Dot(c.X, c.Y + 2), Tiles.RandomFloor());
			}
			
			if (Rnd.Chance()) {
				placed = true;
				Painter.Set(level, new Dot(c.X - 2, c.Y), Tiles.RandomFloor());
			}
			
			if (!placed || Rnd.Chance()) {
				Painter.Set(level, new Dot(c.X + 2, c.Y), Tiles.RandomFloor());
			}
			
			Painter.Set(level, new Dot(c.X, c.Y), Tile.FloorD); // Tiles.RandomFloor());
			
			MobRegistry.SetupForBiome(Biome.Tech);
			Painter.PlaceMobs(level, room);
		}

		public override float GetWeightModifier() {
			return 0.25f;
		}

		public override int GetMinWidth() {
			return 20;
		}

		public override int GetMinHeight() {
			return 20;
		}

		public override int GetMaxWidth() {
			return 30;
		}

		public override int GetMaxHeight() {
			return 30;
		}
	}
}