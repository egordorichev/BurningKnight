using BurningKnight.entity.creature.fx;
using BurningKnight.entity.creature.inventory;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class Grandma : Mob {
		public static Animation Animations = Animation.Make("actor-grandma", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private PointLight Light;
		private AnimationData Tss;

		public Grandma() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 10;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Tss = GetAnimation().Get("tss");
				Animation = Idle;
				W = 16;
				IgnoreRooms = true;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(2, 0, 12, 14, BodyDef.BodyType.DynamicBody, false);
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
			if (Random.Chance(0.1f)) Flipped = !Flipped;

			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("tss"))
				Animation = Tss;
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

			if (!State.Equals("tss")) Animation.Update(Dt);

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
				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}

				case "tss": {
					return new TssState();
				}
			}

			return base.GetAi(State);
		}

		public override void RenderShadow() {
		}

		public override float GetOx() {
			return 8;
		}

		public class GrandmaState : State<Grandma> {
		}

		public class IdleState : GrandmaState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 10f) {
					T = Random.NewFloat(0, 5f);
					AddZ();
				}

				if (Self.Target != null && Self.Target.Room == Self.Room && UiInventory.JustUsed > 0) Self.Become("tss");
			}

			private void AddZ() {
				for (var I = 0; I < 3; I++) {
					var Z = new Zzz();
					Z.X = Self.X + 6;
					Z.Y = Self.Y + 12;
					Z.Delay = I * 0.25f;
					Dungeon.Area.Add(Z);
				}
			}
		}

		public class TssState : GrandmaState {
			public override void OnEnter() {
				base.OnEnter();

				if (Self.NoticeSignT == 0) Self.NoticeSignT = 1f;

				BulletProjectile Bullet = new BookBullet();
				Bullet.Bad = true;
				Bullet.Owner = Self;
				Bullet.X = Self.X + Self.W / 2;
				Bullet.Y = Self.Y + Self.H / 2;
				var A = Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2);
				var D = 60f;
				Bullet.Velocity.X = Math.Cos(A) * D;
				Bullet.Velocity.Y = Math.Sin(A) * D;
				Dungeon.Area.Add(Bullet);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T < 1f)
					Tss.SetFrame((int) (T * 5));
				else
					Self.Become("idle");
			}
		}
	}
}