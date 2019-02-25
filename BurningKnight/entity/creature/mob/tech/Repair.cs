using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.tech {
	public class Repair : Bot {
		public static Animation Animations = Animation.Make("actor-repair", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Move;
		private DeathData TargetDead;

		public Repair() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			W = 9;
			Body = CreateSimpleBody(1, 4, 6, 10, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("move"))
				Animation = Move;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void RenderShadow() {
			Graphics.Shadow(X, Y, 8, H, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			W = 16;
			H = 16;
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

		public class RepairState : State<Repair> {
		}

		public class IdleState : RepairState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room && Data.Size() > 0) Self.Become("move");
			}
		}

		public class MoveState : RepairState {
			public override void OnEnter() {
				base.OnEnter();
				Self.TargetDead = Data.Get(0);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				var Tar = new Point(Self.TargetDead.X, Self.TargetDead.Y);

				if (Self.CanSeePoint(Tar)) NextPathPoint = Tar;

				if (MoveTo(Tar, 20f, 8f)) Self.Become("fix");
			}
		}

		public class FixState : RepairState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) Self.Become("idle");
			}

			public override void OnExit() {
				base.OnExit();

				try {
					var Mob = (Mob) Self.TargetDead.Type.NewInstance();
					Mob.X = Self.TargetDead.X;
					Mob.Y = Self.TargetDead.Y;
					Mob.NoLoot = true;
					Dungeon.Area.Add(Mob.Add());
					Mob.Poof();
				}
				catch (InstantiationException) {
					E.PrintStackTrace();
				}
				catch (IllegalAccessException) {
					E.PrintStackTrace();
				}

				Data.Remove(Self.TargetDead);
				Self.TargetDead = null;
			}
		}
	}
}