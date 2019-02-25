using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.axe;
using BurningKnight.game.input;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.weapon.projectile {
	public class AxeProjectile : Projectile {
		private float A;
		public Axe Axe;
		private bool Did;
		public bool Penetrates;
		public TextureRegion Region;
		public int Speed;

		private float T;
		public Class Type;

		public AxeProjectile() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 17;
			}
		}

		public override void Init() {
			float Dx = Input.Instance.WorldMouse.X - this.X - 8;
			float Dy = Input.Instance.WorldMouse.Y - this.Y - 8;
			var A = (float) Math.Atan2(Dy, Dx);
			var S = Speed * 0.5f;
			Velocity = new Point((float) Math.Cos(A) * S, (float) Math.Sin(A) * S);
			Body = World.CreateSimpleCentredBody(this, 0, 0, Region.GetRegionWidth(), Region.GetRegionHeight(), BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Body.SetBullet(true);
			this.A = Random.NewFloat(Math.PI * 2);
		}

		protected override bool Hit(Entity Entity) {
			if (!Penetrates && Did) return false;

			if (Bad) {
				if (Entity is Player) {
					DoHit(Entity);
					Did = true;

					return true;
				}
			}
			else if (Entity is Mob) {
				DoHit(Entity);
				Did = true;
			}
			else if (Entity is Player && T > 0.2f) {
				Done = true;
			}

			return false;
		}

		public override void Logic(float Dt) {
			Velocity.Mul(0.97f);
			T += Dt;
			this.X += Velocity.X * Dt;
			this.Y += Velocity.Y * Dt;
			A += Dt * 1000;
			World.CheckLocked(Body).SetTransform(this.X, this.Y, (float) Math.ToRadians(A));
			var Dx = Owner.X + Owner.W / 2 - this.X - 8;
			var Dy = Owner.Y + Owner.H / 2 - this.Y - 8;
			var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

			if (D > 8) {
				var F = 6 * Dt * 60;

				if (D < 64 && T > 1) F = MathUtils.Clamp(1f, 10f, 64 - D);

				Velocity.X += Dx / D * F;
				Velocity.Y += Dy / D * F;
			}

			World.CheckLocked(Body).SetTransform(Body.GetPosition(), (float) Math.ToRadians(A));
			Body.SetLinearVelocity(Velocity);
		}

		public override void Render() {
			Graphics.Render(Region, this.X, this.Y, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false);
		}
	}
}