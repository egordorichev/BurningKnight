namespace BurningKnight.entity.item.weapon.projectile {
	public class BookBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			LightUp = false;
			Second = false;
			Rotates = true;
			RotationSpeed = 0.3f;
			Sprite = Graphics.GetTexture("bullet-book");
		}
	}
}