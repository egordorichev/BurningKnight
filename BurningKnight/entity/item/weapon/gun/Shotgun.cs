using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.gun {
	public class Shotgun : Gun {
		public Shotgun() {
			_Init();
		}

		protected void _Init() {
			{
				Accuracy = 20f;
				UseTime = 3f;
				Damage = 3;
				Sprite = "item-gun_a";
				Vel = 3f;
			}
		}

		public override void SendBullets() {
			var Aim = Owner.GetAim();
			var An = Owner.GetAngleTo(Aim.X, Aim.Y);
			var A = An - Math.PI * 2;

			for (var I = 0; I < 4; I++) this.SendBullet(A + Math.ToRadians(Random.NewFloat(-this.Accuracy, this.Accuracy)));
		}

		protected override string GetSfx() {
			return "gun_3";
		}
	}
}