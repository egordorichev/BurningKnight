using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.entity.room.input;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.entity.room.controller {
	public class SpikeFieldController : RoomController {
		private byte variant;
		private bool on = true;
		
		public override void Generate() {
			base.Generate();
			variant = (byte) Random.Int(8);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte(variant);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			variant = stream.ReadByte();
		}

		public override void HandleInputChange(RoomInput.ChangedEvent e) {
			base.HandleInputChange(e);
			on = !on;
		}

		public override void Update(float dt) {
			base.Update(dt);

			foreach (var c in Room.Controllable) {
				if (c is Spikes) {
					if (on) {

						float x = (int) (c.X / 16);
						float y = (int) (c.Y / 16);

						var f = 0f;

						if (variant < 2) {
							f = (x + y) / 3;
						} else if (variant < 4) {
							f = (x - y) / 3;
						} else if (variant < 6) {
							f = x / 3;
						} else {
							f = y / 3;
						}

						c.SetState(((int) (f + T * 0.5f * (variant % 2 == 0 ? 1 : -1)) % 2) == 0);
					} else {
						c.SetState(false);
					}
				}
			}
		}
	}
}