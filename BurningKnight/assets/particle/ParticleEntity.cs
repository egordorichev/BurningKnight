using BurningKnight.entity;
using BurningKnight.entity.component;
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

			if (Depth == Layers.FloorParticles || Depth == Layers.FlyingMob) {
				Depth = Particle.Z > 1 ? Layers.FlyingMob : Layers.FloorParticles;
			}

			Position = Particle.Position;
		}

		public override void Render() {
			base.Render();
			
			if (!Particle.Done) {
				Particle.Renderer.Render(Particle);				
			}
		}

		public void AddShadow() {
			AddComponent(new ShadowComponent(RenderShadow));
		}

		private void RenderShadow() {
			if (!Particle.Done) {
				Particle.Renderer.RenderShadow(Particle);
			}
		}
	}
}