using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.tech {
	public class Repair : Bot {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Move = GetAnimation().Get("move");
				Animation = Idle;
			}
		}

		public class RepairState : Mob.State<Repair>  {

		}

		public class IdleState : RepairState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room && Bot.Data.Size() > 0) {
					Self.Become("move");
				} 
			}
		}

		public class MoveState : RepairState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.TargetDead = Bot.Data.Get(0);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Point Tar = new Point(Self.TargetDead.X, Self.TargetDead.Y);

				if (Self.CanSeePoint(Tar)) {
					NextPathPoint = Tar;
				} 

				if (MoveTo(Tar, 20f, 8f)) {
					Self.Become("fix");
				} 
			}
		}

		public class FixState : RepairState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) {
					Self.Become("idle");
				} 
			}

			public override Void OnExit() {
				base.OnExit();

				try {
					Mob Mob = (Mob) Self.TargetDead.Type.NewInstance();
					Mob.X = Self.TargetDead.X;
					Mob.Y = Self.TargetDead.Y;
					Mob.NoLoot = true;
					Dungeon.Area.Add(Mob.Add());
					Mob.Poof();
				} catch (InstantiationException) {
					E.PrintStackTrace();
				} catch (IllegalAccessException) {
					E.PrintStackTrace();
				}

				Bot.Data.Remove(Self.TargetDead);
				Self.TargetDead = null;
			}
		}

		public static Animation Animations = Animation.Make("actor-repair", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Move;
		private AnimationData Animation;
		private DeathData TargetDead;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			W = 9;
			this.Body = this.CreateSimpleBody(1, 4, 6, 10, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.State.Equals("move")) {
				this.Animation = Move;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X, Y, 8, H, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			W = 16;
			H = 16;
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
				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}

				case "move": {
					return new MoveState();
				}

				case "fix": {
					return new FixState();
				}
			}

			return base.GetAi(State);
		}

		public override float GetOx() {
			return 4;
		}

		public Repair() {
			_Init();
		}
	}
}
