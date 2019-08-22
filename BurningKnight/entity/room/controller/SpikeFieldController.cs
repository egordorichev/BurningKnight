using BurningKnight.entity.room.controllable.spikes;

namespace BurningKnight.entity.room.controller {
	public class SpikeFieldController : RoomController {
		public override void Update(float dt) {
			base.Update(dt);

			foreach (var c in Room.Controllable) {
				if (c is Spikes) {
					float x = (int) (c.X / 16);
					float y = (int) (c.Y / 16);

					var on = (int) ((int) (((x + y) / 3) + T * 0.5f) % 2) == 0;
					c.SetState(on);
				}
			}
		}
	}
}