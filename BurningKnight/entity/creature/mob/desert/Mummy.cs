using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.desert {
	public class Mummy : Mob {
		public static Animation Animations = Animation.Make("actor-mummy", "-white");
		private AnimationData Animation;
		protected List<Player> Colliding = new List<>();
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private float LastHit;
		protected float Mod = 1f;
		private AnimationData Run;
		protected float SpeedModifer = 1f;

		public Mummy() {
			_Init();
		}

		protected void _Init() {
			{
				W = 12;
				HpMax = 10;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_mummy");
			DeathEffect(Killed);
		}

		protected override void OnHurt(int A, Entity From) {
			base.OnHurt(A, From);
			PlaySfx("damage_mummy");
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

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(2, 1, 8, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Speed = 100;
			MaxSpeed = 100;
		}

		public override void Destroy() {
			base.Destroy();
		}

		public override void Render() {
			float V = Math.Abs(Acceleration.X) + Math.Abs(Acceleration.Y);

			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (V > 1f)
				Animation = Run;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Freezed) return;

			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

			if (Dead) {
				Common();

				return;
			}

			LastHit += Dt;

			if (LastHit > 1f)
				foreach (Player Player in Colliding)
					Player.ModifyHp(-1, this);

			if (Animation != null) Animation.Update(Dt * SpeedMod);

			Common();
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) {
				Colliding.Add((Player) Entity);
				LastHit = 0;
				((Player) Entity).ModifyHp(-1, this);
				((Player) Entity).KnockBackFrom(this, 2f * Mod);
				Collide((Player) Entity);
			}
		}

		protected void Collide(Player Player) {
		}

		public override void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) Colliding.Remove(Entity);
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = base.GetDrops();

			return Items;
		}

		public class MummyState : State<Mummy> {
		}

		public class ChaseState : MummyState {
			private float Delay;
			private float Speed;
			private Point To;

			public override void OnEnter() {
				base.OnEnter();
				Speed = Random.NewFloat(15, 25f);
				Delay = Random.NewFloat(15, 25);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.LastSeen != null)
					if (MoveTo(Self.LastSeen, Speed * SpeedModifer, 4f))
						if (Self.Target == null) {
							Self.NoticeSignT = 0f;
							Self.HideSignT = 2f;
							Self.Become("idle");
						}

				if (T >= Delay) Self.Become("tired");
			}
		}

		public class TiredState : MummyState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(3, 5);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("chase");
			}
		}
	}
}