using System;
using System.Collections.Generic;
using Lens.entity;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle {
	public class ParticleSystem : Entity {
		public List<Particle> Particles = new List<Particle>();
		public Func<Particle> Create;
		
		public ParticleSystem(Func<Particle> create, int min, int max, Vector2 position) {
			Create = create;
			Position = position;

			var count = Random.Int(min, max + 1);

			for (int i = 0; i < count; i++) {
				AddParticle();
			}
		}

		public void AddParticle() {
			var particle = Create();
			particle.Controller.Init(particle, this);
			
			Particles.Add(particle);
		}

		public override void Update(float dt) {
			for (int i = Particles.Count - 1; i >= 0; i--) {
				var particle = Particles[i];

				if (particle.Controller.Update(particle, dt)) {
					Particles.RemoveAt(i);
				}
			}

			if (Particles.Count == 0) {
				Done = true;
			}
		}

		public override void Render() {
			foreach (var particle in Particles) {
				particle.Renderer.Render(particle);
			}
		}
	}
}