using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class RektBullet : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			NoRotation = false;
			Sprite = Graphics.GetTexture("bullet-rekt");
		}
	}
}
