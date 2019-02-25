using BurningKnight.core.entity.level.builders;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.regular.boss;

namespace BurningKnight.core.entity.level.levels.hall {
	public class HallBossLevel : HallLevel {
		public override bool Same(Level Level) {
			return base.Same(Level) || Level is HallLevel;
		}

		protected override List CreateRooms<Room> () {
			List<Room> Rooms = new List<>();
			Rooms.Add(this.Entrance = new EntranceRoom());
			Rooms.Add(new HallBossRoom());
			Rooms.Add(this.Exit = new EntranceRoom());
			((EntranceRoom) this.Exit).Exit = true;

			return Rooms;
		}

		protected override Builder GetBuilder() {
			return new LineBuilder();
		}
	}
}
