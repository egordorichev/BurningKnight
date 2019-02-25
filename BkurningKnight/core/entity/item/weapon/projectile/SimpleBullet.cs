using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class SimpleBullet : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			Sprite = Graphics.GetTexture("bullet-a");
		}
	}
}
