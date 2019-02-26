using BurningKnight.entity.level.builders;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.rooms.regular.boss;

namespace BurningKnight.entity.level.levels.hall {
	public class HallBossLevel : HallLevel {
		public override bool Same(Level Level) {
			return base.Same(Level) || Level is HallLevel;
		}

		protected override List CreateRooms<Room>() {
			List<Room> Rooms = new List<>();
			Rooms.Add(Entrance = new EntranceRoom());
			Rooms.Add(new HallBossRoom());
			Rooms.Add(Exit = new EntranceRoom());
			((EntranceRoom) Exit).Exit = true;

			return Rooms;
		}

		protected override Builder GetBuilder() {
			return new LineBuilder();
		}
	}
}