using BurningKnight.entity.room.input;

namespace BurningKnight.entity.room.controller {
	public class TrapRoomController : RoomController {
		public override void Init() {
			base.Init();
			CheckState();
		}

		public override void HandleInputChange(RoomInput.ChangedEvent e) {
			base.HandleInputChange(e);
			CheckState();
		}

		private void CheckState() {
			var on = true;

			foreach (var c in Room.Inputs) {
				if (c.On != c.DefaultState) {
					on = false;
					break;
				}
			}
			
			foreach (var c in Room.Controllable) {
				if (on) {
					c.TurnOn();
				} else {
					c.TurnOff();
				}
			}
		}
	}
}