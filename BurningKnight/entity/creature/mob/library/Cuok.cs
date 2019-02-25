using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class Cuok : Mob {
		public static Animation Animations = Animation.Make("actor-cuok", "-red");
		private AnimationData Animation;
		private AnimationData Attack;
		private AnimationData Bet;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private PointLight Light;
		public bool ToAttack;
		private float Tt;

		public Cuok() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Bet = GetAnimation().Get("anim");
				Attack = GetAnimation().Get("rage");
				Animation = Idle;
				W = 14;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void RenderShadow() {
			Graphics.Shadow(X + 1, Y, W, H, Z + 8);
		}

		public override void Init() {
			base.Init();
			Tt = Random.NewFloat(8);
			Light = World.NewLight(32, new Color(1, 1, 1, 1f), 64, X, Y);
			Light.SetIgnoreAttachedBody(true);
			Flying = true;
			Body = CreateSimpleBody(0, 0, 14, 16, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("attack"))
				Animation = Attack;
			else if (State.Equals("anim"))
				Animation = Bet;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Light.SetActive(true);
			Light.AttachToBody(Body, 8, 8, 0);
			Light.SetPosition(X + 8, Y + 8);
			Light.SetDistance(64);
			Tt += Dt;
			Z = (float) Math.Cos(Tt * 3) * 2;
			Animation.Update(Dt);
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
				case "anim": {
					return new AnimState();
				}

				case "preattack": {
					return new PreattackState();
				}

				case "attack": {
					return new AttackState();
				}

				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public float CalcAngle(float A, int Num) {
			return (float) Math.ToRadians(Math.Round(A / 90f) * 90f);
		}

		public bool Recalc() {
			return false;
		}

		public void SpawnBullet(float A) {
			BulletProjectile Bullet = Random.Chance(50) ? new NanoBullet() : new BulletRect();
			Bullet.Bad = true;
			Bullet.Owner = this;
			Bullet.X = X + W / 2;
			Bullet.Y = Y + 4;
			Bullet.RenderCircle = false;
			var D = 40f;
			Bullet.Velocity.X = Math.Cos(A) * D;
			Bullet.Velocity.Y = Math.Sin(A) * D;
			Dungeon.Area.Add(Bullet);
		}

		public int GetMax() {
			return 5;
		}

		public class CuokState : State<Cuok> {
		}

		public class AttackState : CuokState {
			private float A;
			private int Num;

			public override void OnEnter() {
				base.OnEnter();
				A = (float) Math.ToDegrees(Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2));
				A = Self.CalcAngle(A, 0);
				Num = -1;
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Num == -1) {
					if (T >= 1f) {
						Num = 0;
						T = 0;
					}

					return;
				}

				if (T >= 0.4f) {
					T = 0;

					if (Num == GetMax()) {
						Self.Become("anim");

						return;
					}

					if (Recalc()) {
						A = (float) Math.ToDegrees(Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2));
						A = Self.CalcAngle(A, Num);
					}

					Self.SpawnBullet(A);
					Num++;
				}
			}
		}

		public class AnimState : CuokState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 0.15f) {
					Self.Become(Self.ToAttack ? "attack" : "preattack");
					Self.ToAttack = false;
				}
			}
		}

		public class PreattackState : CuokState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1f, 3f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Delay < T) {
					Self.ToAttack = true;
					Self.Become("anim");
				}
			}
		}

		public class IdleState : CuokState {
			public override void Update(float Dt) {
				if (Self.Target != null && Self.Target.Room == Self.Room) Self.Become("preattack");
			}
		}
	}
}