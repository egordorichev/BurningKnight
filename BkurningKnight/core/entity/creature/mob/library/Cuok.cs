using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.library {
	public class Cuok : Mob {
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

		public class CuokState : Mob.State<Cuok>  {

		}

		public class AttackState : CuokState {
			private int Num;
			private float A;

			public override Void OnEnter() {
				base.OnEnter();
				A = (float) Math.ToDegrees(Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2));
				A = Self.CalcAngle(A, 0);
				Num = -1;
			}

			public override Void Update(float Dt) {
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
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 0.15f) {
					Self.Become(Self.ToAttack ? "attack" : "preattack");
					Self.ToAttack = false;
				} 
			}
		}

		public class PreattackState : CuokState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1f, 3f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Delay < T) {
					Self.ToAttack = true;
					Self.Become("anim");
				} 
			}
		}

		public class IdleState : CuokState {
			public override Void Update(float Dt) {
				if (Self.Target != null && Self.Target.Room == Self.Room) {
					Self.Become("preattack");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-cuok", "-red");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Bet;
		private AnimationData Attack;
		private AnimationData Animation;
		private PointLight Light;
		private float Tt;
		public bool ToAttack;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X + 1, Y, W, H, Z + 8);
		}

		public override Void Init() {
			base.Init();
			Tt = Random.NewFloat(8);
			Light = World.NewLight(32, new Color(1, 1, 1, 1f), 64, X, Y);
			Light.SetIgnoreAttachedBody(true);
			Flying = true;
			this.Body = this.CreateSimpleBody(0, 0, 14, 16, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.State.Equals("attack")) {
				this.Animation = Attack;
			} else if (this.State.Equals("anim")) {
				this.Animation = Bet;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			Light.SetActive(true);
			Light.AttachToBody(Body, 8, 8, 0);
			Light.SetPosition(X + 8, Y + 8);
			Light.SetDistance(64);
			Tt += Dt;
			Z = (float) Math.Cos(Tt * 3) * 2;
			Animation.Update(Dt);
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

		public Void SpawnBullet(float A) {
			BulletProjectile Bullet = Random.Chance(50) ? new NanoBullet() : new BulletRect();
			Bullet.Bad = true;
			Bullet.Owner = this;
			Bullet.X = X + W / 2;
			Bullet.Y = Y + 4;
			Bullet.RenderCircle = false;
			float D = 40f;
			Bullet.Velocity.X = (float) (Math.Cos(A) * D);
			Bullet.Velocity.Y = (float) (Math.Sin(A) * D);
			Dungeon.Area.Add(Bullet);
		}

		public int GetMax() {
			return 5;
		}

		public Cuok() {
			_Init();
		}
	}
}
