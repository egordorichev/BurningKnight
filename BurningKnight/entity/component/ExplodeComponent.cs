using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ExplodeComponent : Component {
		public float Timer;

		public ExplodeComponent(float time) {
			Timer = time;
		}

		public override void Update(float dt) {
			base.Update(dt);

			Timer -= dt;

			if (Timer <= 0) {
				Entity.Done = true;
				// todo: spawn an explosion and hurt everyone around
			}
		}
	}
}