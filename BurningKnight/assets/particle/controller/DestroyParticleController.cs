using System;
using Lens.entity;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.controller {
	public class DestroyParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			particle.Position = owner.Center;
			particle.AngleVelocity = Random.Float(0.6f, 1) * Random.Float(24, 48) * (Random.Chance() ? -1 : 1);

			var a = particle.Angle;
			var f = Random.Float(0.6f, 1f) * Random.Float(80, 120f);

			particle.Angle = 0;
			particle.Velocity = new Vector2((float) Math.Cos(a) * f, (float) Math.Sin(a) * f);
		}
		
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Angle += particle.AngleVelocity * dt;
			particle.Position += particle.Velocity * dt;

			particle.AngleVelocity -= particle.AngleVelocity * dt * 4;
			particle.Velocity -= particle.Velocity * dt * 2;
			
			particle.Update(dt);

			if (particle.T >= 30f) {
				particle.Alpha -= dt * 0.3f;

				if (particle.Alpha <= 0) {
					return true;
				}
			}
			
			return false;
		}
	}
}