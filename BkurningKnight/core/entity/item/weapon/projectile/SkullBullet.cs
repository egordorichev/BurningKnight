using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class SkullBullet : BulletProjectile {
		protected override Void Setup() {
			base.Setup();
			this.Rotates = false;
			this.Second = false;
			this.NoRotation = true;
			this.RectShape = false;
			this.CircleShape = true;
			this.RenderCircle = false;
			this.DissappearWithTime = true;
			this.LightUp = false;
			Sprite = Graphics.GetTexture("bullet-skull");
		}
	}
}
