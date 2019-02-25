using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class Shell : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private static Animation Animations = Animation.Make("fx-shell");
		private TextureRegion Sprite;
		public Point Vel;
		private float Z = 10f;
		private Body Body;
		private float Va;
		private float A;
		private float Al = 1f;
		private float T;
		private bool Tweened;

		public override Void Init() {
			base.Init();
			this.Depth = -1;
			this.Va = Random.NewFloat(this.Vel.X * 10);
			List<Animation.Frame> Frames = Animations.GetFrames("idle");
			this.Sprite = Frames.Get(Random.NewInt(Frames.Size())).Frame;

			if (Settings.Quality > 1) {
				this.Body = World.CreateSimpleCentredBody(this, 0, 0, this.Sprite.GetRegionWidth(), this.Sprite.GetRegionHeight(), BodyDef.BodyType.DynamicBody, false);
				World.CheckLocked(this.Body).SetTransform(this.X + this.Sprite.GetRegionWidth() / 2, this.Y + this.Sprite.GetRegionHeight() / 2, 0);
				this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
				this.Body.SetBullet(true);
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Body != null) {
				this.Vel.X = this.Body.GetLinearVelocity().X;
				this.Vel.Y = this.Body.GetLinearVelocity().Y;
			} else {
				this.T += Dt;

				if (this.T >= 10f && !this.Tweened) {
					this.Tweened = true;
					Tween.To(new Tween.Task(0, 3f, Tween.Type.QUAD_IN) {
						public override float GetValue() {
							return Al;
						}

						public override Void SetValue(float Value) {
							Al = Value;
						}

						public override Void OnEnd() {
							base.OnEnd();
							Done = true;
						}
					});
				} 
			}


			this.Vel.X -= this.Vel.X * Math.Min(1, this.Z == 0 ? Dt * 2 : Dt * 3);
			this.Va -= Va * Math.Min(1, Z == 0 ? Dt * 2 : Dt * 3);
			this.A += this.Va * Dt * 60;

			if (this.Vel.X <= 0.1f && this.Z == 0) {
				this.Vel.X = 0;

				if (this.Body != null) {
					PlaySfx("shell");
					this.Body = World.RemoveBody(this.Body);
				} 
			} 

			this.X += this.Vel.X;
			this.Z = Math.Max(0, this.Z + this.Vel.Y * Dt * 60);
			this.Vel.Y -= Dt * 5;

			if (this.Body != null) {
				this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0);
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(1, 1, 1, this.Al);
			Graphics.Render(this.Sprite, this.X, this.Y + this.Z, this.A, this.Sprite.GetRegionWidth() / 2, this.Sprite.GetRegionHeight() / 2, false, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity != null) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public Shell() {
			_Init();
		}
	}
}
