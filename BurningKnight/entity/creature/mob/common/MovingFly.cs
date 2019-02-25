using BurningKnight.entity.creature.player;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.common {
	public class MovingFly : Fly {
		public static Animation Animations = Animation.Make("actor-fly", "-red");

		public MovingFly() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 5;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 0.3f;
		}

		protected override State GetAi(string State) {
			return new FollowState();
		}

		public class FollowState : State<MovingFly> {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room) FlyTo(Player.Instance, 2f, 4f);

				if (Self.Room != null && Player.Instance.Room == Self.Room)
					foreach (Mob Mob in All)
						if (Mob != Self && Mob.Room == Self.Room && Mob is Fly) {
							var X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
							var Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
							var D = Self.GetDistanceTo(X, Y);

							if (D < 16) {
								var A = D <= 1 ? Random.NewFloat(Math.PI * 2) : Self.GetAngleTo(X, Y);
								var F = 500 * Dt;
								Self.Velocity.X -= Math.Cos(A) * F;
								Self.Velocity.Y -= Math.Sin(A) * F;
							}
						}
			}
		}
	}
}