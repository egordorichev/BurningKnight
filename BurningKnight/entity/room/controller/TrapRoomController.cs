using BurningKnight.entity.creature.player;
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
			var on = Room.Inputs.Count == 0;

			foreach (var c in Room.Inputs) {
				if (c.On == c.DefaultState) {
					on = true;
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

			if (!on) {
				Room.CheckCleared(LocalPlayer.Locate(Room.Area));
			}
		}
	}
}