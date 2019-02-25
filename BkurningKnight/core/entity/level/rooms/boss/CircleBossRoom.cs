using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.boss {
	public class CircleBossRoom : BossRoom {
		protected void _Init() {
			{
				AlwaysElipse = true;
			}
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public CircleBossRoom() {
			_Init();
		}
	}
}
