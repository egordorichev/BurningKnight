using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.boss {
	public class CircleBossRoom : BossRoom {
		public CircleBossRoom() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysElipse = true;
			}
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}
	}
}