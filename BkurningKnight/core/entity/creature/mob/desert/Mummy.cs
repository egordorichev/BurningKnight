using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.desert {
	public class Mummy : Mob {
		protected void _Init() {
			{
				W = 12;
				HpMax = 10;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = this.Idle;
			}
		}

		public class MummyState : Mob.State<Mummy>  {

		}

		public class ChaseState : MummyState {
			private float Delay;
			private Point To;
			private float Speed;

			public override Void OnEnter() {
				base.OnEnter();
				Speed = Random.NewFloat(15, 25f);
				this.Delay = Random.NewFloat(15, 25);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.LastSeen != null) {
					if (this.MoveTo(Self.LastSeen, Speed * SpeedModifer, 4f)) {
						if (Self.Target == null) {
							Self.NoticeSignT = 0f;
							Self.HideSignT = 2f;
							Self.Become("idle");
						} 
					} 
				} 

				if (this.T >= this.Delay) {
					Self.Become("tired");
				} 
			}
		}

		public class TiredState : MummyState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(3, 5);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= this.Delay) {
					Self.Become("chase");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-mummy", "-white");
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;
		private float LastHit;
		protected List<Player> Colliding = new List<>();
		protected float Mod = 1f;
		protected float SpeedModifer = 1f;

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_mummy");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity From) {
			base.OnHurt(A, From);
			this.PlaySfx("damage_mummy");
		}

		public static Mummy Random() {
			return new Mummy();
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "alerted": 
				case "chase": 
				case "roam": {
					return new ChaseState();
				}

				case "idle": 
				case "tired": {
					return new TiredState();
				}
			}

			return base.GetAi(State);
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 1, 8, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Speed = 100;
			MaxSpeed = 100;
		}

		public override Void Destroy() {
			base.Destroy();
		}

		public override Void Render() {
			float V = Math.Abs(this.Acceleration.X) + Math.Abs(this.Acceleration.Y);

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (V > 1f) {
				this.Animation = Run;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Freezed) {
				return;
			} 

			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} 

			if (this.Dead) {
				base.Common();

				return;
			} 

			this.LastHit += Dt;

			if (this.LastHit > 1f) {
				foreach (Player Player in this.Colliding) {
					Player.ModifyHp(-1, this);
				}
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			base.Common();
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) {
				Colliding.Add((Player) Entity);
				LastHit = 0;
				((Player) Entity).ModifyHp(-1, this);
				((Player) Entity).KnockBackFrom(this, 2f * Mod);
				Collide((Player) Entity);
			} 
		}

		protected Void Collide(Player Player) {

		}

		public override Void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) {
				Colliding.Remove(Entity);
			} 
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();

			return Items;
		}

		public Mummy() {
			_Init();
		}
	}
}
