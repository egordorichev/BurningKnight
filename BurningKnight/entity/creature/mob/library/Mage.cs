using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.pattern;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class Mage : Mob {
		public static Animation Animations = Animation.Make("actor-mage", "-yellow");
		private AnimationData Animation;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private PointLight Light;
		private float Lm;

		public Mage() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(7, 0, 7, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Light = World.NewLight(32, new Color(1, 1, 1, 1f), 64, X, Y);
			Light.SetIgnoreAttachedBody(true);
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (State.Equals("roam") || State.Equals("alerted") || State.Equals("idle") || State.Equals("unactive")) return;

			if (Target != null) {
				Flipped = Target.X < this.X;
			}
			else {
				if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;
			}


			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("dissappear"))
				Animation = Dissappear;
			else if (State.Equals("appear"))
				Animation = Appear;
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
			Light.SetDistance(64 * Lm);

			if (!(State.Equals("dissappear") || State.Equals("appear"))) Animation.Update(Dt);

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

		public override void RenderShadow() {
			if (!State.Equals("unactive")) base.RenderShadow();
		}

		public BulletProjectile NewProjectile() {
			BulletProjectile Bullet = new NanoBullet();
			Bullet.Damage = 1;
			Bullet.Owner = this;
			Bullet.Bad = true;
			float A = 0;
			Bullet.X = X;
			Bullet.Y = Y;
			Bullet.Velocity.X = (float) Math.Cos(A);
			Bullet.Velocity.Y = (float) Math.Sin(A);

			return Bullet;
		}

		public class MageState : State<Mage> {
		}

		public class AppearState : MageState {
			public override void Update(float Dt) {
				base.Update(Dt);
				Lm = T * 0.5f;

				if (T < 2f) {
					Self.Appear.SetFrame((int) (T * 5));
				}
				else {
					Self.Idle.SetFrame(0);
					Self.Become("attack");
				}
			}
		}

		public class DissappearState : MageState {
			public override void Update(float Dt) {
				base.Update(Dt);
				Lm = 1 - T * 0.7f;

				if (T < 1.4f)
					Self.Dissappear.SetFrame((int) (T * 5));
				else
					Self.Become("unactive");
			}
		}

		public class WaitState : MageState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.5f, 2f);
				Self.Unhittable = true;
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("appear");
			}

			public override void OnExit() {
				base.OnExit();
				Self.Unhittable = false;

				if (Self.Room != null)
					for (var I = 0; I < 100; I++) {
						var Point = Self.Room.GetRandomFreeCell();

						if (Point != null) {
							if (Player.Instance.GetDistanceTo(Point.X * 16, Point.Y * 16) < 24) continue;

							Self.Tp(Point.X * 16 - 4, Point.Y * 16);

							break;
						}
					}
			}
		}

		public class AttackState : MageState {
			private bool Attacked;

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target.Room != Self.Room) {
					Self.Become("dissappear");

					return;
				}

				if (T >= 2f && !Attacked) {
					Attacked = true;
					var Pattern = new CircleBulletPattern();
					Pattern.Radius = 8f;

					for (var I = 0; I < 5; I++) Pattern.AddBullet(NewProjectile());

					BulletPattern.Fire(Pattern, Self.X + 10, Self.Y + 8, Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8), 40f);
				}

				if (T >= 5f && Self.Idle.GetFrame() == 0) Self.Become("dissappear");
			}
		}

		public class IdleState : MageState {
			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) Self.Become("appear");
			}
		}
	}
}