using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.entities.fx {
	public class PoofFx : Entity {
		private static List<Animation.Frame> Animations = Animation.Make("poof-particles", "-particles").GetFrames("idle");
		private float A;
		private float Max;
		public TextureRegion Region;
		public bool Shadow = true;
		public float Speed = 1f;
		public float T;
		private float Va;
		public Point Vel;

		public PoofFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = -1;
			}
		}

		public override void Init() {
			AlwaysActive = true;

			if (Vel == null) Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f, 1f));

			Vel.Mul(60);
			A = Random.NewFloat(360);
			Va = Random.NewFloat(-10, 10);
			Region = Animations.Get(Random.NewInt(Animations.Size())).Frame;
			Max = Random.NewFloat(1f, 3f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Vel.Mul(0.95f);
			T += Dt;
			A += Va * Dt * 60;
			Va *= 0.995f;

			if (T >= Max) Done = true;
		}

		public override void Render() {
			var S = 1f - T / Max;
			Graphics.Render(Region, this.X, this.Y, A, 10, 10, false, false, S, S);
		}
	}
}