using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class Shotgun : Gun {
		protected void _Init() {
			{
				Accuracy = 20f;
				UseTime = 3f;
				Damage = 3;
				Sprite = "item-gun_a";
				Vel = 3f;
			}
		}

		public override Void SendBullets() {
			Point Aim = this.Owner.GetAim();
			float An = this.Owner.GetAngleTo(Aim.X, Aim.Y);
			float A = (float) (An - Math.PI * 2);

			for (int I = 0; I < 4; I++) {
				this.SendBullet((float) (A + Math.ToRadians(Random.NewFloat(-this.Accuracy, this.Accuracy))));
			}
		}

		protected override string GetSfx() {
			return "gun_3";
		}

		public Shotgun() {
			_Init();
		}
	}
}
