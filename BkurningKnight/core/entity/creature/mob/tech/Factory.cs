using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.pool;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.tech {
	public class Factory : Bot {
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

		public class IdleState : Mob.State<Factory>  {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(12f, 20f);
			}

			public override Void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					T += Dt;

					if (T >= Delay) {
						Self.Become("spawn");
					} 
				} else {
					T = 0;
				}

			}
		}

		public class SpawnState : Mob.State<Factory>  {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Spawn.SetFrame(0);
				Self.Spawn.SetAutoPause(true);
				Self.Spawn.SetPaused(false);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Spawn.IsPaused() && T >= 0.4f) {
					Self.Poof();
					Self.Become("idle");
					Mob Mob = GenerateMob();
					Mob.X = Self.X + (Self.W - Mob.W) / 2;
					Mob.Y = Self.Y - 1;
					Mob.NoLoot = true;
					Dungeon.Area.Add(Mob.Add());
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-factory", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Spawn;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(1, 0, (int) W - 2, (int) H - 2, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
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
			} else if (this.State.Equals("spawn")) {
				this.Animation = Spawn;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
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

		public override Void KnockBackFrom(Entity From, float Force) {

		}

		public static Mob GenerateMob() {
			MobPool.Instance.InitForFloor();
			MobHub Hub = MobPool.Instance.Generate();

			foreach (Class Type in Hub.Types) {
				if (Type == Factory.GetType()) {
					return GenerateMob();
				} 
			}

			try {
				return (Mob) Hub.Types.Get(Random.NewInt(Hub.Types.Size())).NewInstance();
			} catch (InstantiationException) {
				E.PrintStackTrace();
			} catch (IllegalAccessException) {
				E.PrintStackTrace();
			}

			return new MovingFly();
		}

		public Factory() {
			_Init();
		}
	}
}
