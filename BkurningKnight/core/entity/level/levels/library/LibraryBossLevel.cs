using BurningKnight.core.entity.level.builders;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.regular.boss;

namespace BurningKnight.core.entity.level.levels.library {
	public class LibraryBossLevel : LibraryLevel {
		protected override List CreateRooms<Room> () {
			List<Room> Rooms = new List<>();
			Rooms.Add(this.Entrance = new EntranceRoom());
			Rooms.Add(new LibraryBossRoom());
			Rooms.Add(this.Exit = new EntranceRoom());
			((EntranceRoom) this.Exit).Exit = true;

			return Rooms;
		}

		protected override Builder GetBuilder() {
			return new LineBuilder();
		}
	}
}
