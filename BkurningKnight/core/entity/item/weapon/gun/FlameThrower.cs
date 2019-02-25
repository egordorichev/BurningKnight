using BurningKnight.core.entity.fx;
using BurningKnight.core.game.input;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class FlameThrower : Gun {
		protected void _Init() {
			{
				Sprite = "item-gun_a";
				AmmoMax = 8;
			}
		}

		private float LastAmmo;
		private float Last;

		public override Void Use() {
			this.Down = true;
		}

		public override Void Update(float Dt) {
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
					float X = this.Owner.X + this.Owner.W / 2 + (Flipped ? -7 : 7);
					float Y = this.Owner.Y + this.Owner.H / 4 + this.Owner.Z;
					X += this.GetAimX(0, 0);
					Y += this.GetAimY(0, 0);
					FireFx Fx = Random.Chance(50) ? new FireFxPhysic() : new FireFx();
					Fx.X = X + Random.NewFloat(-4, 4);
					Fx.Y = Y + Random.NewFloat(-4, 4);
					float F = 120f;
					Fx.Vel.X = (float) (Math.Cos(this.LastAngle) * F);
					Fx.Vel.Y = (float) (Math.Sin(this.LastAngle) * F);
					Dungeon.Area.Add(Fx);
				} 

				if (Input.Instance.WasReleased("use")) {
					this.Down = false;
				} 
			} 
		}

		public FlameThrower() {
			_Init();
		}
	}
}
