using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmEndRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.Center = room.Center;
		}

		public override void PlacePlayer(Room room, Player player) {
			player.Center = room.Center + new Vector2(0, 32);
		}

		public override void Paint(Level level, Room room) {
			
		}
	}
}