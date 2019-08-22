using Lens.entity;

namespace BurningKnight.entity.room.controllable.platform {
	public class SteppingPlatform : MovingPlatform {
		public override void Init() {
			base.Init();
			On = false;
		}

		protected override string GetAnimation() {
			return "stepping_platform";
		}

		public override bool HandleEvent(Event e) {
			if (e is StartedSupportingEvent) {
				if (!On) {
					TurnOn();
				}
			} else if (e is EndedSupportingEvent) {
				if (On && Supporting.Count == 0) {
					TurnOff();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}