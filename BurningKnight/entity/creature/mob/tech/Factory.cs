using BurningKnight.entity.creature.mob.common;
using BurningKnight.entity.pool;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.tech {
	public class Factory : Bot {
		public static Animation Animations = Animation.Make("actor-factory", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Spawn;

		public Factory() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Spawn = GetAnimation().Get("spawn");
				Animation = Idle;
				W = 25;
				H = 19;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(1, 0, (int) W - 2, (int) H - 2, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
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
			else if (State.Equals("spawn"))
				Animation = Spawn;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
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
				case "alerted":
				case "roam": {
					return new IdleState();
				}

				case "spawn": {
					return new SpawnState();
				}
			}

			return base.GetAi(State);
		}

		public override void KnockBackFrom(Entity From, float Force) {
		}

		public static Mob GenerateMob() {
			MobPool.Instance.InitForFloor();
			var Hub = MobPool.Instance.Generate();

			foreach (Class Type in Hub.Types)
				if (Type == Factory.GetType())
					return GenerateMob();

			try {
				return (Mob) Hub.Types.Get(Random.NewInt(Hub.Types.Size())).NewInstance();
			}
			catch (InstantiationException) {
				E.PrintStackTrace();
			}
			catch (IllegalAccessException) {
				E.PrintStackTrace();
			}

			return new MovingFly();
		}

		public class IdleState : State<Factory> {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(12f, 20f);
			}

			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					T += Dt;

					if (T >= Delay) Self.Become("spawn");
				}
				else {
					T = 0;
				}
			}
		}

		public class SpawnState : State<Factory> {
			public override void OnEnter() {
				base.OnEnter();
				Self.Spawn.SetFrame(0);
				Self.Spawn.SetAutoPause(true);
				Self.Spawn.SetPaused(false);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Spawn.IsPaused() && T >= 0.4f) {
					Self.Poof();
					Self.Become("idle");
					var Mob = GenerateMob();
					Mob.X = Self.X + (Self.W - Mob.W) / 2;
					Mob.Y = Self.Y - 1;
					Mob.NoLoot = true;
					Dungeon.Area.Add(Mob.Add());
				}
			}
		}
	}
}