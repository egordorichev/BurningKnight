using BurningKnight.core.entity.level.builders;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.regular.boss;

namespace BurningKnight.core.entity.level.levels.desert {
	public class DesertBossLevel : DesertLevel {
		protected override List CreateRooms<Room> () {
			List<Room> Rooms = new List<>();
			Rooms.Add(this.Entrance = new EntranceRoom());
			Rooms.Add(new DesertBossRoom());
			Rooms.Add(this.Exit = new EntranceRoom());
			((EntranceRoom) this.Exit).Exit = true;

			return Rooms;
		}

		public override bool Same(Level Level) {
			return base.Same(Level) || Level is DesertLevel;
		}

		protected override Builder GetBuilder() {
			return new LineBuilder();
		}
	}
}
