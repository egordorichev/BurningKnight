using System;
using Lens.entity;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.controller {
	public class BkDeathParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			base.Init(particle, owner);

			var a = Random.AnglePI();
			var d = Random.Float(60, 90);
			
			particle.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
		}

		public override bool Update(Particle particle, float dt) {
			particle.Position += particle.Velocity * dt;
			particle.Velocity -= particle.Velocity * (dt * 2);
			particle.Alpha -= dt;

			if (particle.Alpha <= 0) {
				return true;
			}

			return base.Update(particle, dt);
		}
	}
}