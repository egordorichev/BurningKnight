using BurningKnight.core.entity.creature.player;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.common {
	public class MovingFly : Fly {
		protected void _Init() {
			{
				HpMax = 5;
			}
		}

		public class FollowState : Mob.State<MovingFly>  {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room) {
					FlyTo(Player.Instance, 2f, 4f);
				} 

				if (Self.Room != null && Player.Instance.Room == Self.Room) {
					foreach (Mob Mob in Mob.All) {
						if (Mob != Self && Mob.Room == Self.Room && Mob is Fly) {
							float X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
							float Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
							float D = Self.GetDistanceTo(X, Y);

							if (D < 16) {
								float A = D <= 1 ? Random.NewFloat((float) (Math.PI * 2)) : Self.GetAngleTo(X, Y);
								float F = 500 * Dt;
								Self.Velocity.X -= Math.Cos(A) * F;
								Self.Velocity.Y -= Math.Sin(A) * F;
							} 
						} 
					}
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-fly", "-red");

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 0.3f;
		}

		protected override State GetAi(string State) {
			return new FollowState();
		}

		public MovingFly() {
			_Init();
		}
	}
}
