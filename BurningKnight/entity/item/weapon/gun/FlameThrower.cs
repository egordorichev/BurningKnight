using BurningKnight.entity.fx;
using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.gun {
	public class FlameThrower : Gun {
		private float Last;

		private float LastAmmo;

		public FlameThrower() {
			_Init();
		}

		protected void _Init() {
			{
				Sprite = "item-gun_a";
				AmmoMax = 8;
			}
		}

		public override void Use() {
			this.Down = true;
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (this.Down) {
				Last += Dt;
				LastAmmo += Dt;

				if (LastAmmo >= 0.4f) {
					LastAmmo = 0;
					AmmoLeft = Math.Max(0, AmmoLeft - 1);

					if (AmmoLeft == 0) {
						Down = false;

						return;
					}
				}

				if (Last >= 0.05f) {
					Last = 0;
					var X = Owner.X + Owner.W / 2 + (Flipped ? -7 : 7);
					var Y = Owner.Y + Owner.H / 4 + Owner.Z;
					X += this.GetAimX(0, 0);
					Y += this.GetAimY(0, 0);
					var Fx = Random.Chance(50) ? new FireFxPhysic() : new FireFx();
					Fx.X = X + Random.NewFloat(-4, 4);
					Fx.Y = Y + Random.NewFloat(-4, 4);
					var F = 120f;
					Fx.Vel.X = Math.Cos(this.LastAngle) * F;
					Fx.Vel.Y = Math.Sin(this.LastAngle) * F;
					Dungeon.Area.Add(Fx);
				}

				if (Input.Instance.WasReleased("use")) this.Down = false;
			}
		}
	}
}