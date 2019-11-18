using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle {
	public class Particle {
		public ParticleController Controller;
		public ParticleRenderer Renderer;
		
		public Vector2 Position;
		public Vector2 Velocity;

		public float Z;
		public float Zv;
		
		public float Angle;
		public float AngleVelocity;

		public float T;
		public float Alpha = 1f;
		public float Scale = 1f;
		public bool Done;
		public int Rnd;

		public Particle(ParticleController controller, ParticleRenderer renderer) {
			Controller = controller;
			Renderer = renderer;

			Angle = Lens.util.math.Rnd.AnglePI();
			Rnd = Lens.util.math.Rnd.Int(1024);
			T = Lens.util.math.Rnd.Float(1f);
		}

		public virtual void Update(float dt) {
			
		}
	}
}