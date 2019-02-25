namespace BurningKnight.entity.item.weapon.projectile {
	public class NanoBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			NoRotation = true;
			Second = false;
			Sprite = Graphics.GetTexture("bullet-nano");
		}
	}
}