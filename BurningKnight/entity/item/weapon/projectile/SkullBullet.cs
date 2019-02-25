namespace BurningKnight.entity.item.weapon.projectile {
	public class SkullBullet : BulletProjectile {
		protected override void Setup() {
			base.Setup();
			Rotates = false;
			Second = false;
			NoRotation = true;
			RectShape = false;
			CircleShape = true;
			RenderCircle = false;
			DissappearWithTime = true;
			LightUp = false;
			Sprite = Graphics.GetTexture("bullet-skull");
		}
	}
}