using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.physics;

namespace BurningKnight.entity.fx {
	public class BallProjectile : Projectile {
		private static TextureRegion Sprite = Graphics.GetTexture("item-bullet_e");

		public BallProjectile() {
			_Init();
		}

		protected void _Init() {
			{
				W = Sprite.GetRegionWidth();
				H = Sprite.GetRegionHeight();
				IgnoreVel = true;
				Damage = 4;
			}
		}

		protected override bool Hit(Entity Entity) {
			if (Entity is Mob) return true;

			return base.Hit(Entity);
		}

		public override void Init() {
			base.Init();
			Body = World.CreateCircleBody(this, 0, 0, W / 2, BodyDef.BodyType.DynamicBody, false, 0.8f);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			MassData Data = new MassData();
			Data.Mass = 0.01f;
			Body.SetMassData(Data);
		}

		public override void Render() {
			Graphics.Render(Sprite, this.X, this.Y);
		}
	}
}