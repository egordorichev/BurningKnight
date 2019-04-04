using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ExplodeComponent : Component {
		public float Timer;
		public float Radius = 32f;

		public override void Update(float dt) {
			base.Update(dt);

			Timer -= dt;

			if (Timer <= 0) {
				Entity.Done = true;
				ExplosionMaker.Make(Entity, Radius);
			}
		}
	}
}