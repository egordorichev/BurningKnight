using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class KotlinBullet : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			this.Second = false;
			LightUp = false;
			Sprite = Graphics.GetTexture("bullet-kotlin");
		}
	}
}
