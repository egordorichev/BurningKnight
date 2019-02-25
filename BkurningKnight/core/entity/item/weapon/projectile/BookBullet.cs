using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class BookBullet : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			LightUp = false;
			Second = false;
			Rotates = true;
			RotationSpeed = 0.3f;
			Sprite = Graphics.GetTexture("bullet-book");
		}
	}
}
