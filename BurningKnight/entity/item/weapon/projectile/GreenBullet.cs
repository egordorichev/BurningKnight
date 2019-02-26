namespace BurningKnight.entity.item.weapon.projectile {
	public class GreenBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			Sprite = Graphics.GetTexture("bullet-c");
		}
	}
}