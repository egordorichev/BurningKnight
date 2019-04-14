using BurningKnight.entity;
using Lens.entity;

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
			
			Particle.Controller.Init(Particle, this);

			AlwaysActive = true;
			AlwaysVisible = true;

			Depth = Layers.Door;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Particle.Controller.Update(Particle, dt) || Particle.Done) {
				Done = true;
			}
		}

		public override void Render() {
			base.Render();
			
			if (!Particle.Done) {
				Particle.Renderer.Render(Particle);				
			}
		}
	}
}