namespace BurningKnight.entity.item.weapon.projectile {
	public class KotlinBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			Second = false;
			LightUp = false;
			Sprite = Graphics.GetTexture("bullet-kotlin");
		}
	}
}