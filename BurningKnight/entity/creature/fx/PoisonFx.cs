using BurningKnight.entity.creature.buff;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.fx {
	public class PoisonFx : Entity {
		private static List<Animation.Frame> Animations = Animation.Make("poison").GetFrames("idle");
		private float A;
		private float Al;
		private float B;
		private Body Body;
		private float G;
		private float R;
		private TextureRegion Region;
		private float T;

		public PoisonFx() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -9;
			}
		}

		public override void Init() {
			base.Init();
			R = Random.NewFloat(0f, 0.05f);
			A = Random.NewFloat(360);
			G = Random.NewFloat(0.4f, 0.8f);
			B = Random.NewFloat(0, 0.05f);
			Region = Animations.Get(Random.NewInt(Animations.Size())).Frame;
			Body = World.CreateCircleCentredBody(this, 0, 0, 12, BodyDef.BodyType.StaticBody, true);
			World.CheckLocked(Body).SetTransform(this.X + 16, this.Y + 16, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature && !((Creature) Entity).IsFlying()) ((Creature) Entity).AddBuff(new PoisonedBuff().SetDuration(2f));
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (T >= 10f) {
				Al = Math.Max(0f, Al - Dt);

				if (Al == 0) Done = true;
			}
			else if (Al < 1f) {
				Al = Math.Min(1f, Al + Dt * 3f);
			}
		}

		public override void Render() {
			Graphics.Batch.SetColor(R, G, B, 0.8f);
			Graphics.Render(Region, this.X + 16, this.Y + 16, A, 16, 16, false, false, Al, Al);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}