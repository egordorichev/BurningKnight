using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.biome;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmEnemyRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.Center = room.Center;
		}

		public override void PlacePlayer(Room room, Player player) {
			player.Center = room.Center + new Vector2(0, 32);
		}

		public override void Paint(Level level, Room room) {
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