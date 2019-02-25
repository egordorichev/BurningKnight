using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.pattern;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.library {
	public class Mage : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Appear = GetAnimation().Get("appear");
				Dissappear = GetAnimation().Get("dissappear");
				Animation = Idle;
				W = 21;
			}
		}

		public class MageState : Mob.State<Mage>  {

		}

		public class AppearState : MageState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				Lm = T * 0.5f;

				if (T < 2f) {
					Self.Appear.SetFrame((int) (T * 5));
				} else {
					Self.Idle.SetFrame(0);
					Self.Become("attack");
				}

			}
		}

		public class DissappearState : MageState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				Lm = 1 - T * 0.7f;

				if (T < 1.4f) {
					Self.Dissappear.SetFrame((int) (T * 5));
				} else {
					Self.Become("unactive");
				}

			}
		}

		public class WaitState : MageState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.5f, 2f);
				Self.Unhittable = true;
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					Self.Become("appear");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Self.Unhittable = false;

				if (Self.Room != null) {
					for (int I = 0; I < 100; I++) {
						Point Point = Self.Room.GetRandomFreeCell();

						if (Point != null) {
							if (Player.Instance.GetDistanceTo(Point.X * 16, Point.Y * 16) < 24) {
								continue;
							} 

							Self.Tp(Point.X * 16 - 4, Point.Y * 16);

							break;
						} 
					}
				} 
			}
		}

		public class AttackState : MageState {
			private bool Attacked;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target.Room != Self.Room) {
					Self.Become("dissappear");

					return;
				} 

				if (T >= 2f && !Attacked) {
					Attacked = true;
					CircleBulletPattern Pattern = new CircleBulletPattern();
					Pattern.Radius = 8f;

					for (int I = 0; I < 5; I++) {
						Pattern.AddBullet(NewProjectile());
					}

					BulletPattern.Fire(Pattern, Self.X + 10, Self.Y + 8, Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8), 40f);
				} 

				if (T >= 5f && Self.Idle.GetFrame() == 0) {
					Self.Become("dissappear");
				} 
			}
		}

		public class IdleState : MageState {
			public override Void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					Self.Become("appear");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-mage", "-yellow");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Animation;
		private PointLight Light;
		private float Lm;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(7, 0, 7, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Light = World.NewLight(32, new Color(1, 1, 1, 1f), 64, X, Y);
			Light.SetIgnoreAttachedBody(true);
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (State.Equals("roam") || State.Equals("alerted") || State.Equals("idle") || State.Equals("unactive")) {
				return;
			} 

			if (this.Target != null) {
				this.Flipped = this.Target.X < this.X;
			} else {
				if (Math.Abs(this.Velocity.X) > 1f) {
					this.Flipped = this.Velocity.X < 0;
				} 
			}


			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.State.Equals("dissappear")) {
				this.Animation = Dissappear;
			} else if (this.State.Equals("appear")) {
				this.Animation = Appear;
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
			Light.SetDistance(64 * Lm);

			if (!(State.Equals("dissappear") || State.Equals("appear"))) {
				Animation.Update(Dt);
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
				case "appear": {
					return new AppearState();
				}

				case "dissappear": {
					return new DissappearState();
				}

				case "unactive": {
					return new WaitState();
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

		public override Void RenderShadow() {
			if (!State.Equals("unactive")) {
				base.RenderShadow();
			} 
		}

		public BulletProjectile NewProjectile() {
			BulletProjectile Bullet = new NanoBullet();
			Bullet.Damage = 1;
			Bullet.Owner = this;
			Bullet.Bad = true;
			float A = 0;
			Bullet.X = X;
			Bullet.Y = Y;
			Bullet.Velocity.X = (float) (Math.Cos(A));
			Bullet.Velocity.Y = (float) (Math.Sin(A));

			return Bullet;
		}

		public Mage() {
			_Init();
		}
	}
}
