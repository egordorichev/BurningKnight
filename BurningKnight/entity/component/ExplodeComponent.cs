using System;
using BurningKnight.assets.particle;
using Lens.entity.component;
using Lens.util.camera;
using Microsoft.Xna.Framework;

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
				Camera.Instance.Shake(10);
				
				var explosion = new ParticleEntity(Particles.Animated("explosion", "explosion"));
				explosion.Particle.Position = Entity.Center;
				explosion.Depth = 32;
				Entity.Area.Add(explosion);

				for (int i = 0; i < 4; i++) {
					explosion = new ParticleEntity(Particles.Animated("explosion", "smoke"));
					explosion.Particle.Position = Entity.Center;
					explosion.Depth = 31;
					Entity.Area.Add(explosion);

					var a = explosion.Particle.Angle - Math.PI / 2;
					var d = 16;

					explosion.Particle.Position += new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				}
			}
		}
	}
}