using BurningKnight.core.assets;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.entities.fx {
	public class PoofFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = -1;
			}
		}

		private static List<Animation.Frame> Animations = Animation.Make("poof-particles", "-particles").GetFrames("idle");
		public TextureRegion Region;
		public Point Vel;
		public float Speed = 1f;
		public bool Shadow = true;
		public float T;
		private float A;
		private float Va;
		private float Max;

		public override Void Init() {
			this.AlwaysActive = true;

			if (this.Vel == null) {
				this.Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f, 1f));
			} 

			this.Vel.Mul(60);
			A = Random.NewFloat(360);
			Va = Random.NewFloat(-10, 10);
			Region = Animations.Get(Random.NewInt(Animations.Size())).Frame;
			Max = Random.NewFloat(1f, 3f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Vel.Mul(0.95f);
			this.T += Dt;
			this.A += this.Va * Dt * 60;
			this.Va *= 0.995f;

			if (this.T >= Max) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			float S = 1f - this.T / Max;
			Graphics.Render(Region, this.X, this.Y, this.A, 10, 10, false, false, S, S);
		}

		public PoofFx() {
			_Init();
		}
	}
}
