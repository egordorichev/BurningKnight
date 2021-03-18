using System;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.controller {
	public class DestroyParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			particle.Position = owner.Center;
			particle.AngleVelocity = Rnd.Float(0.6f, 1) * Rnd.Float(24, 48) * (Rnd.Chance() ? -1 : 1);

			var a = particle.Angle;
			var f = Rnd.Float(0.6f, 1f) * Rnd.Float(80, 120f);

			particle.Angle = 0;
			particle.Velocity = new Vector2((float) Math.Cos(a) * f, (float) Math.Sin(a) * f);
			particle.Zv = Rnd.Float(1, 3);

			particle.T = Rnd.Float(1f);
		}
		
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Angle += particle.AngleVelocity * dt;
			particle.Position += particle.Velocity * dt;

			particle.AngleVelocity -= particle.AngleVelocity * dt * 4;
			particle.Velocity -= particle.Velocity * dt * 2;

			particle.Z += particle.Zv * dt * 60;

			if (particle.Z <= 0) {
				particle.Z = 0;
				particle.Zv = 0;
				particle.Velocity.X = 0;
				particle.Velocity.Y = 0;
			} else {
				particle.Zv -= dt * 5;
			}

			particle.Update(dt);

			if (particle.T >= 5f) {
				particle.Alpha -= dt * 0.3f;

				if (particle.Alpha <= 0) {
					return true;
				}
			}
			
			return false;
		}
	}
}