using BurningKnight.entity.creature.fx;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.active;
using BurningKnight.entity.item.entity;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.hall {
	public class Clown : Mob {
		public static Animation Animations = Animation.Make("actor-clown", "-purple");
		private AnimationData Animation;
		private Guitar Guitar;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private bool PlacedBomb;
		private AnimationData Run;
		private bool SpawnBomb;

		public Clown() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 6;
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

		public override float GetWeight() {
			return 0.7f;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Speed = 100;
			MaxSpeed = 100;
			Guitar = new Guitar();
			Guitar.SetOwner(this);
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(15)) Items.Add(new Bomb());

			if (Random.Chance(1)) Items.Add(new Guitar());

			if (Random.Chance(1)) Items.Add(new InfiniteBomb());

			return Items;
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		public override bool RollBlock() {
			Become("chase");

			return base.RollBlock();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			Done = true;
			DeathEffect(Killed);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (SpawnBomb) {
				SpawnBomb = false;
				var E = new BombEntity(this.X, this.Y).VelTo(Player.Instance.X + 8, Player.Instance.Y + 8, 60f);
				PlacedBomb = true;
				Apply(E);
				Dungeon.Area.Add(E);

				foreach (Mob Mob in All)
					if (Mob.Room == Room)
						Mob.Become("getout");
			}

			if (Freezed) return;

			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

			if (Dead) {
				Common();

				return;
			}

			if (Animation != null) Animation.Update(Dt * SpeedMod);

			if (Guitar != null) Guitar.Update(Dt * SpeedMod);

			Common();
		}

		public override void Render() {
			Graphics.Batch.SetColor(1, 1, 1, A);
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

			if (Guitar != null) {
				Graphics.Batch.SetColor(1, 1, 1, A);
				Guitar.Render(this.X, this.Y, W, H, Flipped, false);
			}

			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "getout": {
					return new GetOutState();
				}

				case "idle": {
					return new IdleState();
				}

				case "alerted": {
					return new AlertedState();
				}

				case "chase": {
					return new ChasingState();
				}

				case "wait": {
					return new WaitState();
				}

				case "laugh": {
					return new LaughState();
				}

				case "attack": {
					return new AttackState();
				}

				case "roam": {
					return new RoamState();
				}

				case "rangedAttack": {
					return new RangedAttack();
				}
			}

			return base.GetAi(State);
		}

		public override void Destroy() {
			base.Destroy();

			if (Guitar != null) Guitar.Destroy();
		}

		public void Apply(BombEntity Bomb) {
		}

		public class ClownState : State<Clown> {
		}

		public class WaitState : ClownState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1, 3f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("chase");
			}
		}

		public class LaughState : ClownState {
			public override void OnEnter() {
				base.OnEnter();
				Self.PlacedBomb = false;
				Self.PlaySfx("laugh");
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) Self.Become("chase");
			}
		}

		public class IdleState : ClownState {
			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Target != null) {
					var D = Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8);
					Self.Become(D > 32f && Random.Chance(75) ? "rangedAttack" : "chase");
				}
			}
		}

		public class AlertedState : ClownState {
			private const float DELAY = 1f;

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.T >= DELAY) Self.Become("wait");
			}
		}

		public class RangedAttack : ClownState {
			private float LastAttack;

			public override void Update(float Dt) {
				base.Update(Dt);
				LastAttack += Dt;
				CheckForPlayer();

				if (!Self.CanSee(Self.Target))
					if (!MoveTo(Self.LastSeen, 10f, 64f))
						return;

				if (Self.Target != null && LastAttack >= 1f && Random.Chance(75)) {
					var Note = new Note();
					LastAttack = 0;
					var Dx = Self.X + Self.W / 2 - Self.Target.X - Self.Target.W / 2 + Random.NewFloat(-10f, 10f);
					var Dy = Self.Y + Self.H / 2 - Self.Target.Y - Self.Target.H / 2 + Random.NewFloat(-10f, 10f);
					Note.A = (float) Math.Atan2(-Dy, -Dx);
					Note.X = Self.X + Self.W / 2;
					Note.Y = Self.Y + Self.H / 2;
					Note.Bad = !Self.Stupid;
					Dungeon.Area.Add(Note);
				}

				if (Self.Target != null) {
					var D = Self.GetDistanceTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2);

					if (D < 64f) Self.Become("chase");
				}

				if (T >= 3f) {
					Self.Become("chase");

					if (Random.Chance(75)) Self.Become("rangedAttack");
				}
			}
		}

		public class ChasingState : ClownState {
			public const float ATTACK_DISTANCE = 24f;

			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				var D = 32f;

				if (Target != null) D = Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2));

				if (MoveTo(Self.LastSeen, D < 48f ? 30f : 10f, ATTACK_DISTANCE)) Self.Become("attack");


				base.Update(Dt);
			}
		}

		public class GetOutState : Mob.GetOutState {
			protected override string GetState() {
				return ((Clown) Self).PlacedBomb ? "laugh" : "wait";
			}
		}

		public class AttackState : ClownState {
			private int Step;

			public override void OnEnter() {
				base.OnEnter();
			}

			private void DoAttack() {
				if (Step < 3 && false) {
					Self.Guitar.Use();
				}

				var E = new BombEntity(Self.X, Self.Y).VelTo(Self.LastSeen.X + 8, Self.LastSeen.Y + 8, 60f);
				Self.PlacedBomb = true;
				Self.Apply(E);
				Dungeon.Area.Add(E);

				foreach (Mob Mob in All)
					if (Mob.Room == Self.Room)
						Mob.Become("getout");


				Step++;
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				DoAttack();
			}
		}

		public class RoamState : ClownState {
			public override void OnEnter() {
				base.OnEnter();
				FindNearbyPoint();
			}

			public override void Update(float Dt) {
				if (TargetPoint != null && MoveTo(TargetPoint, 6f, 8f)) {
					Self.Become("idle");

					return;
				}

				CheckForPlayer();
				base.Update(Dt);
			}
		}
	}
}