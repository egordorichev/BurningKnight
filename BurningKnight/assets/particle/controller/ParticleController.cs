using System;
using Lens.entity;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.controller {
	public class ParticleController {
		public virtual void Init(Particle particle, Entity owner) {
			particle.Position = owner.Center;
			particle.AngleVelocity = Random.Float(0.6f, 1) * 16 * (Random.Chance() ? -1 : 1);

			var a = Random.AnglePI();
			var f = Random.Float(0.6f, 1f) * 40f;
			
			particle.Velocity = new Vector2((float) Math.Cos(a) * f, (float) Math.Sin(a) * f);
		}
		
		public virtual bool Update(Particle particle, float dt) {
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
			
			particle.Update(dt);
			
			return false;
		}
	}
}