using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.active;
using BurningKnight.core.entity.item.entity;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.hall {
	public class Clown : Mob {
		protected void _Init() {
			{
				HpMax = 6;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = this.Idle;
			}
		}

		public class ClownState : State<Clown>  {

		}

		public class WaitState : ClownState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1, 3f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= Delay) {
					Self.Become("chase");
				} 
			}
		}

		public class LaughState : ClownState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.PlacedBomb = false;
				Self.PlaySfx("laugh");
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) {
					Self.Become("chase");
				} 
			}
		}

		public class IdleState : ClownState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (this.Target != null) {
					float D = Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8);
					Self.Become((D > 32f && Random.Chance(75)) ? "rangedAttack" : "chase");
				} 
			}
		}

		public class AlertedState : ClownState {
			private const float DELAY = 1f;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.T >= DELAY) {
					Self.Become("wait");
				} 
			}
		}

		public class RangedAttack : ClownState {
			private float LastAttack;

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.LastAttack += Dt;
				this.CheckForPlayer();

				if (!Self.CanSee(Self.Target)) {
					if (!this.MoveTo(Self.LastSeen, 10f, 64f)) {
						return;
					} 
				} 

				if (Self.Target != null && this.LastAttack >= 1f && Random.Chance(75)) {
					Note Note = new Note();
					this.LastAttack = 0;
					float Dx = Self.X + Self.W / 2 - Self.Target.X - Self.Target.W / 2 + Random.NewFloat(-10f, 10f);
					float Dy = Self.Y + Self.H / 2 - Self.Target.Y - Self.Target.H / 2 + Random.NewFloat(-10f, 10f);
					Note.A = (float) Math.Atan2(-Dy, -Dx);
					Note.X = Self.X + (Self.W) / 2;
					Note.Y = Self.Y + (Self.H) / 2;
					Note.Bad = !Self.Stupid;
					Dungeon.Area.Add(Note);
				} 

				if (Self.Target != null) {
					float D = Self.GetDistanceTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2);

					if (D < 64f) {
						Self.Become("chase");
					} 
				} 

				if (this.T >= 3f) {
					Self.Become("chase");

					if (Random.Chance(75)) {
						Self.Become("rangedAttack");
					} 
				} 
			}
		}

		public class ChasingState : ClownState {
			public const float ATTACK_DISTANCE = 24f;

			public override Void Update(float Dt) {
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} else {
					float D = 32f;

					if (this.Target != null) {
						D = Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2));
					} 

					if (this.MoveTo(Self.LastSeen, D < 48f ? 30f : 10f, ATTACK_DISTANCE)) {
						Self.Become("attack");
					} 
				}


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

			public override Void OnEnter() {
				base.OnEnter();
			}

			private Void DoAttack() {
				if (this.Step < 3 && false) {
					Self.Guitar.Use();
				} else {
					BombEntity E = new BombEntity(Self.X, Self.Y).VelTo(Self.LastSeen.X + 8, Self.LastSeen.Y + 8, 60f);
					Self.PlacedBomb = true;
					Self.Apply(E);
					Dungeon.Area.Add(E);

					foreach (Mob Mob in Mob.All) {
						if (Mob.Room == Self.Room) {
							Mob.Become("getout");
						} 
					}
				}


				this.Step++;
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				DoAttack();
			}
		}

		public class RoamState : ClownState {
			public override Void OnEnter() {
				base.OnEnter();
				this.FindNearbyPoint();
			}

			public override Void Update(float Dt) {
				if (this.TargetPoint != null && this.MoveTo(this.TargetPoint, 6f, 8f)) {
					Self.Become("idle");

					return;
				} 

				this.CheckForPlayer();
				base.Update(Dt);
			}
		}

		public static Animation Animations = Animation.Make("actor-clown", "-purple");
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;
		private Guitar Guitar;
		private bool SpawnBomb;
		private bool PlacedBomb;

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 0.7f;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Speed = 100;
			MaxSpeed = 100;
			this.Guitar = new Guitar();
			this.Guitar.SetOwner(this);
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(15)) {
				Items.Add(new Bomb());
			} 

			if (Random.Chance(1)) {
				Items.Add(new Guitar());
			} 

			if (Random.Chance(1)) {
				Items.Add(new InfiniteBomb());
			} 

			return Items;
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		public override bool RollBlock() {
			this.Become("chase");

			return base.RollBlock();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			this.Done = true;
			DeathEffect(Killed);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (SpawnBomb) {
				SpawnBomb = false;
				BombEntity E = new BombEntity(this.X, this.Y).VelTo(Player.Instance.X + 8, Player.Instance.Y + 8, 60f);
				this.PlacedBomb = true;
				this.Apply(E);
				Dungeon.Area.Add(E);

				foreach (Mob Mob in Mob.All) {
					if (Mob.Room == this.Room) {
						Mob.Become("getout");
					} 
				}
			} 

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

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			if (Guitar != null) {
				this.Guitar.Update(Dt * SpeedMod);
			} 

			base.Common();
		}

		public override Void Render() {
			Graphics.Batch.SetColor(1, 1, 1, this.A);
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

			if (Guitar != null) {
				Graphics.Batch.SetColor(1, 1, 1, this.A);
				this.Guitar.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
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

		public override Void Destroy() {
			base.Destroy();

			if (Guitar != null) {
				this.Guitar.Destroy();
			} 
		}

		public Void Apply(BombEntity Bomb) {

		}

		public Clown() {
			_Init();
		}
	}
}
