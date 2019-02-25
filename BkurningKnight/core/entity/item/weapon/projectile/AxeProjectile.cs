using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.axe;
using BurningKnight.core.game.input;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class AxeProjectile : Projectile {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 17;
			}
		}

		private float T;
		private float A;
		public TextureRegion Region;
		public bool Penetrates;
		public Class Type;
		public int Speed;
		public Axe Axe;
		private bool Did;

		public override Void Init() {
			float Dx = Input.Instance.WorldMouse.X - this.X - 8;
			float Dy = Input.Instance.WorldMouse.Y - this.Y - 8;
			float A = (float) Math.Atan2(Dy, Dx);
			float S = this.Speed * 0.5f;
			this.Velocity = new Point((float) Math.Cos(A) * S, (float) Math.Sin(A) * S);
			this.Body = World.CreateSimpleCentredBody(this, 0, 0, this.Region.GetRegionWidth(), this.Region.GetRegionHeight(), BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.Body.SetBullet(true);
			this.A = Random.NewFloat((float) (Math.PI * 2));
		}

		protected override bool Hit(Entity Entity) {
			if (!this.Penetrates && this.Did) {
				return false;
			} 

			if (this.Bad) {
				if (Entity is Player) {
					this.DoHit(Entity);
					this.Did = true;

					return true;
				} 
			} else if (Entity is Mob) {
				this.DoHit(Entity);
				this.Did = true;
			} else if (Entity is Player && this.T > 0.2f) {
				this.Done = true;
			} 

			return false;
		}

		public override Void Logic(float Dt) {
			this.Velocity.Mul(0.97f);
			this.T += Dt;
			this.X += this.Velocity.X * Dt;
			this.Y += this.Velocity.Y * Dt;
			this.A += Dt * 1000;
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, (float) Math.ToRadians(this.A));
			float Dx = this.Owner.X + this.Owner.W / 2 - this.X - 8;
			float Dy = this.Owner.Y + this.Owner.H / 2 - this.Y - 8;
			float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

			if (D > 8) {
				float F = 6 * Dt * 60;

				if (D < 64 && this.T > 1) {
					F = MathUtils.Clamp(1f, 10f, 64 - D);
				} 

				this.Velocity.X += Dx / D * F;
				this.Velocity.Y += Dy / D * F;
			} 

			World.CheckLocked(this.Body).SetTransform(this.Body.GetPosition(), (float) Math.ToRadians(this.A));
			this.Body.SetLinearVelocity(this.Velocity);
		}

		public override Void Render() {
			Graphics.Render(this.Region, this.X, this.Y, this.A, this.Region.GetRegionWidth() / 2, this.Region.GetRegionHeight() / 2, false, false);
		}

		public AxeProjectile() {
			_Init();
		}
	}
}
