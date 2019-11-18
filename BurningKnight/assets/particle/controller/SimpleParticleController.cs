using System;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.controller {
	public class SimpleParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			base.Init(particle, owner);
			
			particle.AngleVelocity = Rnd.Float(0.6f, 1) * 16 * (Rnd.Chance() ? -1 : 1);

			var a = Rnd.AnglePI();
			var f = Rnd.Float(0.6f, 1f) * 40f;
			
			particle.Velocity = new Vector2((float) Math.Cos(a) * f, (float) Math.Sin(a) * f);
		}

		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Angle += particle.AngleVelocity * dt;
			particle.Position += particle.Velocity * dt;
			particle.Scale -= dt;

			if (particle.Scale <= 0f) {
				particle.Scale = 0;
				return true;
			}

			particle.AngleVelocity -= particle.AngleVelocity * dt * 4;
			particle.Velocity -= particle.Velocity * dt * 4;
			
			return base.Update(particle, dt);
		}
	}
}