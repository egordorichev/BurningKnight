using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.creature.inventory;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.library {
	public class Grandma : Mob {
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

		public class GrandmaState : Mob.State<Grandma>  {

		}

		public class IdleState : GrandmaState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 10f) {
					T = Random.NewFloat(0, 5f);
					AddZ();
				} 

				if (Self.Target != null && Self.Target.Room == Self.Room && UiInventory.JustUsed > 0) {
					Self.Become("tss");
				} 
			}

			private Void AddZ() {
				for (int I = 0; I < 3; I++) {
					Zzz Z = new Zzz();
					Z.X = Self.X + 6;
					Z.Y = Self.Y + 12;
					Z.Delay = I * 0.25f;
					Dungeon.Area.Add(Z);
				}
			}
		}

		public class TssState : GrandmaState {
			public override Void OnEnter() {
				base.OnEnter();

				if (Self.NoticeSignT == 0) {
					Self.NoticeSignT = 1f;
				} 

				BulletProjectile Bullet = new BookBullet();
				Bullet.Bad = true;
				Bullet.Owner = Self;
				Bullet.X = Self.X + Self.W / 2;
				Bullet.Y = Self.Y + Self.H / 2;
				float A = Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2);
				float D = 60f;
				Bullet.Velocity.X = (float) (Math.Cos(A) * D);
				Bullet.Velocity.Y = (float) (Math.Sin(A) * D);
				Dungeon.Area.Add(Bullet);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T < 1f) {
					Tss.SetFrame((int) (T * 5));
				} else {
					Self.Become("idle");
				}

			}
		}

		public static Animation Animations = Animation.Make("actor-grandma", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Tss;
		private AnimationData Animation;
		private PointLight Light;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 0, 12, 14, BodyDef.BodyType.DynamicBody, false);
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
			if (Random.Chance(0.1f)) {
				Flipped = !Flipped;
			} 

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.State.Equals("tss")) {
				this.Animation = Tss;
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

			if (!State.Equals("tss")) {
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

		public override Void RenderShadow() {

		}

		public override float GetOx() {
			return 8;
		}

		public Grandma() {
			_Init();
		}
	}
}
