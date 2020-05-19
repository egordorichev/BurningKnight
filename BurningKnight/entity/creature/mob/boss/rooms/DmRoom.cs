using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.rooms.regular;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public abstract class DmRoom : RegularRoom {
		public abstract void PlaceMage(Room room, DM mage);
		public abstract void PlacePlayer(Room room, Player player);
		public abstract void Paint(Level level, Room room);
	}
}