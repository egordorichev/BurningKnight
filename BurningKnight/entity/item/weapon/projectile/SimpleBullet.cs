namespace BurningKnight.entity.item.weapon.projectile {
	public class SimpleBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			Sprite = Graphics.GetTexture("bullet-a");
		}
	}
}