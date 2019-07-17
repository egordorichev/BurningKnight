using BurningKnight.assets.particle;
using Lens.entity.component;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class SparkEmitterComponent : Component {
		private readonly float delay;
		private readonly float chance;
		
		private float sinceLast;
		
		public SparkEmitterComponent(float delay, float chance) {
			this.delay = delay;
			this.chance = chance;
		}

		public override void Update(float dt) {
			base.Update(dt);

			sinceLast += dt;

			if (sinceLast > delay) {
				sinceLast = 0;

				if (Random.Chance(chance)) {
					var p = Particles.Wrap(Particles.Spark(), Entity.Area, new Vector2(Random.Float(Entity.Width) + Entity.X,
						Random.Float(Entity.Height) + Entity.Y));

					p.Depth = 1;
				}
			}
		}
	}
}