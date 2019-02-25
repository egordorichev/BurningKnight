using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.prefix {
	public class DeathShotPrefix : Prefix {
		private static Color Color = Color.ValueOf("#ff00ff");

		public override Color GetColor() {
			return Color;
		}

		public override Void OnDeath(Mob Mob) {
			base.OnDeath(Mob);

			for (int I = 0; I < 8; I++) {
				BulletProjectile Ball = new NanoBullet();
				float A = (float) (I * Math.PI / 4);
				Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(60f * Mob.ShotSpeedMod);
				Ball.X = (float) (Mob.X + Mob.W / 2 + Math.Cos(A) * 8);
				Ball.Y = (float) (Mob.Y + Math.Sin(A) * 8 + 6);
				Ball.Damage = 2;
				Ball.Bad = true;
				Dungeon.Area.Add(Ball);
			}
		}
	}
}
