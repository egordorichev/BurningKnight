using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.hall {
	public class RangedKnight : Knight {
		protected void _Init() {
			{
				HpMax = 6;
			}
		}

		public class IdleState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.Target != null) {
					Self.Become("alerted");
				} 
			}
		}

		public class ChaseState : KnightState {
			public override Void Update(float Dt) {
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} else {
					float Att = 180;

					if (this.MoveTo(Self.LastSeen, 18f, Att) && Self.OnScreen) {
						if (Self.Target != null && Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2)) <= Att) {
							if (Self.CanSee(Self.Target)) {
								Self.Become("preattack");
							} 
						} else {
							Self.NoticeSignT = 0f;
							Self.HideSignT = 2f;
							Self.Become("idle");
						}

					} 
				}


				base.Update(Dt);
			}
		}

		public class RunAwayState : KnightState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} 

				this.MoveFrom(Self.LastSeen, 20f, 6f);
				float D = Self.GetDistanceTo(Self.Target.X, Self.Target.Y);

				if (D >= Self.MinAttack) {
					Self.Become("preattack");
				} 
			}
		}

		public class AttackingState : KnightState {
			public override Void OnEnter() {
				base.OnEnter();
			}

			public override Void Update(float Dt) {
				if (!((Gun) Self.Sword).IsReloading()) {
					if (!CanSee(Self.Target) || Self.GetDistanceTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2) > 220f) {
						Self.Become("chase");

						return;
					} 

					Self.Sword.Use();
					Self.Become("preattack");
					CheckForRun();
				} 
			}
		}

		public class PreAttackState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				float Dx = Self.Target.X + Self.Target.W / 2 - LastAim.X;
				float Dy = Self.Target.Y + Self.Target.H / 2 - LastAim.Y;
				float S = 0.04f;
				LastAim.X += Dx * S;
				LastAim.Y += Dy * S;

				if (this.T > 1f) {
					Self.Become("attack");
				} 

				CheckForRun();
			}
		}

		public static Animation Animations = Animation.Make("actor-knight", "-red");
		private Point LastAim = new Point();

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Sword = new BadGun();
			this.Sword.SetOwner(this);
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = new List<>();

			if (Random.Chance(5)) {
				Items.Add(new Revolver());
			} 

			return Items;
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "preattack": {
					return new PreAttackState();
				}

				case "attack": {
					return new AttackingState();
				}

				case "runaway": 
				case "alerted": {
					return new RunAwayState();
				}

				case "roam": 
				case "idle": {
					return new IdleState();
				}

				case "chase": {
					return new ChaseState();
				}
			}

			return base.GetAi(State);
		}

		public override bool RollBlock() {
			return false;
		}

		public Void CheckForRun() {
			if (((Gun) this.Sword).IsReloading()) {
				return;
			} 

			if (this.Target == null) {
				return;
			} 

			float D = this.GetDistanceTo(this.Target.X + this.Target.W / 2, this.Target.Y + this.Target.H / 2);

			if (D < 64f) {
				this.Become("runaway");
			} 
		}

		public override Void Tp(float X, float Y) {
			base.Tp(X, Y);
			LastAim.X = X + 10;
			LastAim.Y = Y;
		}

		public override Point GetAim() {
			return LastAim;
		}

		public RangedKnight() {
			_Init();
		}
	}
}
