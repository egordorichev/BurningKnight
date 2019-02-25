using BurningKnight.core.assets;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.forest {
	public class Wombat : Mob {
		protected void _Init() {
			{
				HpMax = 8;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Sucking = GetAnimation().Get("sucking");
				Blowing = GetAnimation().Get("blowing");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public class WombatState : Mob.State<Wombat>  {

		}

		public class BlowingState : WombatState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Blowing.SetFrame(0);
				float A = (float) (Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8) - Math.PI);
				float Speed = 100f;
				Self.Body.SetLinearVelocity(new Point((float) Math.Cos(A) * Speed, (float) Math.Sin(A) * Speed));
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Vector2 Vel = Self.Body.GetLinearVelocity();

				if (Vel.Len2() < 300f) {
					Vel.X += Vel.X * Dt;
					Vel.Y += Vel.Y * Dt;
					Self.Body.SetLinearVelocity(Vel);
				} 

				if (Random.Chance(30)) {
					bool Atom = Random.Chance(40);
					BulletProjectile Ball = Atom ? new BulletAtom() : new NanoBullet();
					float A = (float) (Math.Atan2(Vel.Y, Vel.X) - Math.PI);
					A += Random.NewFloat(-1, 1);
					Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul((Atom ? 60f : 40f) * Mob.ShotSpeedMod * Random.NewFloat(0.9f, 1.1f));
					Ball.X = (Self.X + (Self.Flipped ? 0 : Self.W));
					Ball.Y = (Self.Y + 4);
					Ball.Damage = 2;
					Ball.RenderCircle = false;
					Ball.Bad = true;
					Dungeon.Area.Add(Ball);
				} 

				if (T >= 6f) {
					Self.Become("tired");
				} else {
					Self.Blowing.SetFrame((int) (T * 0.5f));
				}

			}
		}

		public class TiredState : WombatState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				Vector2 Vel = Self.Body.GetLinearVelocity();
				Vel.X -= Vel.X * Dt * 4f;
				Vel.Y -= Vel.Y * Dt * 4f;
				Self.Body.SetLinearVelocity(Vel);

				if (T >= 3f) {
					Self.Become("sucking");
				} 
			}
		}

		public class SuckingState : WombatState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Sucking.SetFrame(0);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 1f) {
					Self.Become("blowing");
				} else {
					Self.Sucking.SetFrame((int) (T * 3f));
				}

			}
		}

		public class IdleState : WombatState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					Self.Become("sucking");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-wombat", "-brown");
		private AnimationData Idle;
		private AnimationData Sucking;
		private AnimationData Blowing;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			Hx = 4;
			Hy = 3;
			Hw = 12;
			Hh = 9;
			Flying = true;
			this.Body = World.CreateSimpleBody(this, Hx, Hy, Hw, Hh, BodyDef.BodyType.DynamicBody, false, 1);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = State.Equals("blowing") ? this.Velocity.X > 0 : this.Velocity.X < 0;
			} 

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (State.Equals("blowing")) {
				this.Animation = Blowing;
			} else if (State.Equals("sucking")) {
				this.Animation = Sucking;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is SolidProp || Entity is RollingSpike) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && (State.Equals("idle") || State.Equals("tired"))) {
				Animation.Update(Dt);
			} 

			if (Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;
				float A = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
				this.Body.SetLinearVelocity(((float) Math.Cos(A)) * 32 * Mob.SpeedMod + Knockback.X * 0.2f, ((float) Math.Sin(A)) * 32 * Mob.SpeedMod + Knockback.Y * 0.2f);
			} 

			if (Target != null && Target.Room != Room) {
				Become("idle");
			} 

			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "blowing": {
					return new BlowingState();
				}

				case "sucking": {
					return new SuckingState();
				}

				case "tired": {
					return new TiredState();
				}

				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public Wombat() {
			_Init();
		}
	}
}
