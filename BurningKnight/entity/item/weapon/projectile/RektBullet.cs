namespace BurningKnight.entity.item.weapon.projectile {
	public class RektBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			NoRotation = false;
			Sprite = Graphics.GetTexture("bullet-rekt");
		}
	}
}