using BurningKnight.entity;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.assets.particle {
	// Wraps particle for cases, when particle system is too much
	public class ParticleEntity : Entity {
		public Particle Particle;

		public ParticleEntity(Particle particle) {
			Particle = particle;
		}

		public override void Init() {
			base.Init();

			Width = 0;
			Height = 0;

			X += Random.Float(-1, 1);
			Y += Random.Float(-1, 1);
			
			Particle.Controller.Init(Particle, this);
			AlwaysActive = true;

			Depth = Layers.FloorParticles;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Particle.Controller.Update(Particle, dt) || Particle.Done) {
				Done = true;
				return;
			}

			Position = Particle.Position;
		}

		public override void Render() {
			base.Render();
			
			if (!Particle.Done) {
				Particle.Renderer.Render(Particle);				
			}
		}
	}
}