using BurningKnight.entity.room.controllable.turret;

namespace BurningKnight.entity.room.controller {
	public class BossRoomController : RoomController {
		public override void Init() {
			base.Init();

			foreach (var c in Room.Controllable) {
				if (c is Turret t) {
					t.TurnOn();
					t.TimingOffset = 0;
				}
			}
		}
	}
}