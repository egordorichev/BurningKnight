using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.weapon.gun {
	public class Shell : Entity {
		private static Animation Animations = Animation.Make("fx-shell");
		private float A;
		private float Al = 1f;
		private Body Body;
		private TextureRegion Sprite;
		private float T;
		private bool Tweened;
		private float Va;
		public Point Vel;
		private float Z = 10f;

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Depth = -1;
			Va = Random.NewFloat(Vel.X * 10);
			List<Animation.Frame> Frames = Animations.GetFrames("idle");
			Sprite = Frames.Get(Random.NewInt(Frames.Size())).Frame;

			if (Settings.Quality > 1) {
				Body = World.CreateSimpleCentredBody(this, 0, 0, Sprite.GetRegionWidth(), Sprite.GetRegionHeight(), BodyDef.BodyType.DynamicBody, false);
				World.CheckLocked(Body).SetTransform(this.X + Sprite.GetRegionWidth() / 2, this.Y + Sprite.GetRegionHeight() / 2, 0);
				Body.SetLinearVelocity(Vel.X, Vel.Y);
				Body.SetBullet(true);
			}
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Body != null) {
				Vel.X = Body.GetLinearVelocity().X;
				Vel.Y = Body.GetLinearVelocity().Y;
			}
			else {
				T += Dt;

				if (T >= 10f && !Tweened) {
					Tweened = true;
					Tween.To(new Tween.Task(0, 3f, Tween.Type.QUAD_IN) {

		public override float GetValue() {
			return Al;
		}

		public override void SetValue(float Value) {
			Al = Value;
		}

		public override void OnEnd() {
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
public override void Render() {
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