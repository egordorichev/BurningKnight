using BurningKnight.entity.bomb;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ExplodeComponent : Component {
		public float Timer;
		public float Radius = 32f;

		public override void Update(float dt) {
			base.Update(dt);

			Timer -= dt;

			if (Timer <= 0) {
				if (Entity is Bomb b) {
					b.Explode();

					return;
				}
				
				Entity.Done = true;
				ExplosionMaker.Make(Entity, Radius);
			}
		}
	}
}