using Lens.entity;

namespace BurningKnight.assets.particle {
	// Wraps particle for cases, when particle system is too much
	public class ParticleEntity : Entity {
		public Particle Particle;

		public ParticleEntity(Particle particle) {
			Particle = particle;
			Particle.Controller.Init(Particle, this);
		}
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Particle.Controller.Update(Particle, dt)) {
				Done = true;
			}
		}

		public override void Render() {
			base.Render();
			Particle.Renderer.Render(Particle);
		}
	}
}