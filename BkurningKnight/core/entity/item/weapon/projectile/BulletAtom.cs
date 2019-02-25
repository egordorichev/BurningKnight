using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class BulletAtom : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			this.NoRotation = true;
			this.Second = false;
			Sprite = Graphics.GetTexture("bullet-atom");
		}
	}
}
