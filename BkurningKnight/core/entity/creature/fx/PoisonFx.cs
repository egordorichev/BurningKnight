using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.fx {
	public class PoisonFx : Entity {
		protected void _Init() {
			{
				Depth = -9;
			}
		}

		private static List<Animation.Frame> Animations = Animation.Make("poison").GetFrames("idle");
		private float A;
		private float R;
		private float G;
		private float B;
		private TextureRegion Region;
		private float Al;
		private Body Body;
		private float T;

		public override Void Init() {
			base.Init();
			this.R = Random.NewFloat(0f, 0.05f);
			this.A = Random.NewFloat(360);
			this.G = Random.NewFloat(0.4f, 0.8f);
			this.B = Random.NewFloat(0, 0.05f);
			this.Region = Animations.Get(Random.NewInt(Animations.Size())).Frame;
			this.Body = World.CreateCircleCentredBody(this, 0, 0, 12, BodyDef.BodyType.StaticBody, true);
			World.CheckLocked(this.Body).SetTransform(this.X + 16, this.Y + 16, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature && !((Creature) Entity).IsFlying()) {
				((Creature) Entity).AddBuff(new PoisonedBuff().SetDuration(2f));
			} 
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;

			if (this.T >= 10f) {
				this.Al = Math.Max(0f, this.Al - Dt);

				if (this.Al == 0) {
					this.Done = true;
				} 
			} else if (this.Al < 1f) {
				this.Al = Math.Min(1f, this.Al + Dt * 3f);
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(this.R, this.G, this.B, 0.8f);
			Graphics.Render(Region, this.X + 16, this.Y + 16, this.A, 16, 16, false, false, this.Al, this.Al);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public PoisonFx() {
			_Init();
		}
	}
}
