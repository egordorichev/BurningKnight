using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.forest {
	public class Wombat : Mob {
		public static Animation Animations = Animation.Make("actor-wombat", "-brown");
		private AnimationData Animation;
		private AnimationData Blowing;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Sucking;

		public Wombat() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Hx = 4;
			Hy = 3;
			Hw = 12;
			Hh = 9;
			Flying = true;
			Body = World.CreateSimpleBody(this, Hx, Hy, Hw, Hh, BodyDef.BodyType.DynamicBody, false, 1);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Math.Abs(Velocity.X) > 1f) Flipped = State.Equals("blowing") ? Velocity.X > 0 : Velocity.X < 0;

			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("blowing"))
				Animation = Blowing;
			else if (State.Equals("sucking"))
				Animation = Sucking;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is SolidProp || Entity is RollingSpike) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && (State.Equals("idle") || State.Equals("tired"))) Animation.Update(Dt);

			if (Body != null) {
				Velocity.X = Body.GetLinearVelocity().X;
				Velocity.Y = Body.GetLinearVelocity().Y;
				var A = (float) Math.Atan2(Velocity.Y, Velocity.X);
				Body.SetLinearVelocity((float) Math.Cos(A) * 32 * SpeedMod + Knockback.X * 0.2f, (float) Math.Sin(A) * 32 * SpeedMod + Knockback.Y * 0.2f);
			}

			if (Target != null && Target.Room != Room) Become("idle");

			Common();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
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

		public class WombatState : State<Wombat> {
		}

		public class BlowingState : WombatState {
			public override void OnEnter() {
				base.OnEnter();
				Self.Blowing.SetFrame(0);
				var A = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8) - Math.PI;
				var Speed = 100f;
				Self.Body.SetLinearVelocity(new Point((float) Math.Cos(A) * Speed, (float) Math.Sin(A) * Speed));
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Vector2 Vel = Self.Body.GetLinearVelocity();

				if (Vel.Len2() < 300f) {
					Vel.X += Vel.X * Dt;
					Vel.Y += Vel.Y * Dt;
					Self.Body.SetLinearVelocity(Vel);
				}

				if (Random.Chance(30)) {
					var Atom = Random.Chance(40);
					BulletProjectile Ball = Atom ? new BulletAtom() : new NanoBullet();
					var A = (float) (Math.Atan2(Vel.Y, Vel.X) - Math.PI);
					A += Random.NewFloat(-1, 1);
					Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul((Atom ? 60f : 40f) * ShotSpeedMod * Random.NewFloat(0.9f, 1.1f));
					Ball.X = Self.X + (Self.Flipped ? 0 : Self.W);
					Ball.Y = Self.Y + 4;
					Ball.Damage = 2;
					Ball.RenderCircle = false;
					Ball.Bad = true;
					Dungeon.Area.Add(Ball);
				}

				if (T >= 6f)
					Self.Become("tired");
				else
					Self.Blowing.SetFrame((int) (T * 0.5f));
			}
		}

		public class TiredState : WombatState {
			public override void Update(float Dt) {
				base.Update(Dt);
				Vector2 Vel = Self.Body.GetLinearVelocity();
				Vel.X -= Vel.X * Dt * 4f;
				Vel.Y -= Vel.Y * Dt * 4f;
				Self.Body.SetLinearVelocity(Vel);

				if (T >= 3f) Self.Become("sucking");
			}
		}

		public class SuckingState : WombatState {
			public override void OnEnter() {
				base.OnEnter();
				Self.Sucking.SetFrame(0);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 1f)
					Self.Become("blowing");
				else
					Self.Sucking.SetFrame((int) (T * 3f));
			}
		}

		public class IdleState : WombatState {
			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) Self.Become("sucking");
			}
		}
	}
}