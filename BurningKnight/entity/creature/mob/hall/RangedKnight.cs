using BurningKnight.entity.item.weapon.gun;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.hall {
	public class RangedKnight : Knight {
		public static Animation Animations = Animation.Make("actor-knight", "-red");
		private Point LastAim = new Point();

		public RangedKnight() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 6;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			this.Sword = new BadGun();
			this.Sword.SetOwner(this);
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = new List<>();

			if (Random.Chance(5)) Items.Add(new Revolver());

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

		public void CheckForRun() {
			if (((Gun) this.Sword).IsReloading()) return;

			if (Target == null) return;

			var D = GetDistanceTo(Target.X + Target.W / 2, Target.Y + Target.H / 2);

			if (D < 64f) Become("runaway");
		}

		public override void Tp(float X, float Y) {
			base.Tp(X, Y);
			LastAim.X = X + 10;
			LastAim.Y = Y;
		}

		public override Point GetAim() {
			return LastAim;
		}

		public class IdleState : KnightState {
			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null) Self.Become("alerted");
			}
		}

		public class ChaseState : KnightState {
			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				float Att = 180;

				if (MoveTo(Self.LastSeen, 18f, Att) && Self.OnScreen) {
					if (Self.Target != null && Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2)) <= Att) {
						if (Self.CanSee(Self.Target)) Self.Become("preattack");
					}
					else {
						Self.NoticeSignT = 0f;
						Self.HideSignT = 2f;
						Self.Become("idle");
					}
				}


				base.Update(Dt);
			}
		}

		public class RunAwayState : KnightState {
			public override void OnEnter() {
				base.OnEnter();
				Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				MoveFrom(Self.LastSeen, 20f, 6f);
				var D = Self.GetDistanceTo(Self.Target.X, Self.Target.Y);

				if (D >= Self.MinAttack) Self.Become("preattack");
			}
		}

		public class AttackingState : KnightState {
			public override void OnEnter() {
				base.OnEnter();
			}

			public override void Update(float Dt) {
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
			public override void Update(float Dt) {
				base.Update(Dt);
				var Dx = Self.Target.X + Self.Target.W / 2 - LastAim.X;
				var Dy = Self.Target.Y + Self.Target.H / 2 - LastAim.Y;
				var S = 0.04f;
				LastAim.X += Dx * S;
				LastAim.Y += Dy * S;

				if (T > 1f) Self.Become("attack");

				CheckForRun();
			}
		}
	}
}