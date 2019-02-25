using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class IceElemental : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Killed = GetAnimation().Get("dead");
			}
		}

		public class IdleState : Mob.State<IceElemental>  {

		}

		public static Animation Animations = Animation.Make("actor-ice-elemental", "-blue");
		private AnimationData Idle;
		private AnimationData Killed;
		private Body Aura;
		private PointLight Light;
		private float Rad = 24;
		protected bool Stop;
		private Vector2 LastVel;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void KnockBackFrom(Entity From, float Force) {

		}

		public override Void Init() {
			base.Init();
			Idle.Randomize();
			Flying = true;
			this.Body = World.CreateCircleCentredBody(this, 8, 8, 8, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			this.Aura = World.CreateCircleCentredBody(this, 8, 8, 24, BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(this.Aura).SetTransform(this.X, this.Y, 0);
			float F = 32;
			this.Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(this.Velocity);
			Light = World.NewLight(32, Firefly.ColorIce, 64, X, Y);
			Light.SetIgnoreAttachedBody(true);
		}

		public override HpFx ModifyHp(int Amount, Creature From) {
			return null;
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
			this.Aura = World.RemoveBody(this.Aura);
			World.RemoveLight(Light);
		}

		public override float GetOy() {
			return 16;
		}

		public override Void Render() {
			if (Rad > 0) {
				Graphics.StartAlphaShape();
				Graphics.Shape.SetColor(Firefly.ColorIce.R, Firefly.ColorIce.G, Firefly.ColorIce.B, 0.3f);
				Graphics.Shape.Circle(X + 8, Y + 8, Rad);
				Graphics.EndAlphaShape();
			} 

			this.RenderWithOutline(this.Idle);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Die() {
			base.Die();
			Tween.To(new Tween.Task(0, 0.1f) {
				public override float GetValue() {
					return Rad;
				}

				public override Void SetValue(float Value) {
					Rad = Value;
				}
			});
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X, Y, W, H, 10);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			Light.SetActive(true);
			Light.AttachToBody(Body, 0, 0, 0);
			Light.SetPosition(X + 8, Y + 8);
			Light.SetDistance(64);

			if (this.Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;

				if (LastVel == null && Stop) {
					LastVel = new Vector2(Velocity.X, Velocity.Y);
				} else if (!Stop && LastVel != null) {
					Velocity.X = LastVel.X;
					Velocity.Y = LastVel.Y;
					LastVel = null;
				} 

				if (Stop) {
					this.Body.SetLinearVelocity(0, 0);
				} else {
					float A = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
					this.Body.SetLinearVelocity(((float) Math.Cos(A)) * 32 * Mob.SpeedMod, ((float) Math.Sin(A)) * 32 * Mob.SpeedMod);
				}


				World.CheckLocked(this.Aura).SetTransform(this.X, this.Y, 0);
			} 

			Idle.Update(Dt);
			base.Common();

			if (!Dd && Room != null && Room == Player.Instance.Room) {
				foreach (Mob Mob in Mob.All) {
					if (!Mob.IsDead() && Mob.Room == Room) {
						return;
					} 
				}

				Die();
			} 
		}

		public override float GetWeight() {
			return 0.5f;
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			DeathEffect(Killed);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return Entity == null || Entity is Player || Entity is Door || Entity is BulletProjectile;
		}

		protected override State GetAi(string State) {
			return new IdleState();
		}

		public override Void OnCollision(Entity Entity) {
			if (Entity is Player) {
				((Player) Entity).AddBuff(new FrozenBuff().SetDuration(1f));
			} else if (Entity is BulletProjectile && Target != null) {
				BulletProjectile Bullet = (BulletProjectile) Entity;
				float A = this.GetAngleTo(Target.X + 8, Target.Y + 8);
				float F = (float) Math.Sqrt(Bullet.Velocity.X * Bullet.Velocity.X + Bullet.Velocity.Y * Bullet.Velocity.Y);
				Bullet.Velocity.X = (float) (Math.Cos(A) * F);
				Bullet.Velocity.Y = (float) (Math.Sin(A) * F);
				Bullet.Bad = true;

				if (Bullet.Body != null) {
					Bullet.Body.SetLinearVelocity(Bullet.Velocity);
				} 

				Bullet.Angle = (float) Math.ToDegrees(A);
				Bullet.Ra = A;
			} 
		}

		public IceElemental() {
			_Init();
		}
	}
}
