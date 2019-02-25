using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.physics;

namespace BurningKnight.core.entity.fx {
	public class BallProjectile : Projectile {
		protected void _Init() {
			{
				W = Sprite.GetRegionWidth();
				H = Sprite.GetRegionHeight();
				IgnoreVel = true;
				Damage = 4;
			}
		}

		private static TextureRegion Sprite = Graphics.GetTexture("item-bullet_e");

		protected override bool Hit(Entity Entity) {
			if (Entity is Mob) {
				return true;
			} 

			return base.Hit(Entity);
		}

		public override Void Init() {
			base.Init();
			this.Body = World.CreateCircleBody(this, 0, 0, this.W / 2, BodyDef.BodyType.DynamicBody, false, 0.8f);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			MassData Data = new MassData();
			Data.Mass = 0.01f;
			this.Body.SetMassData(Data);
		}

		public override Void Render() {
			Graphics.Render(Sprite, this.X, this.Y);
		}

		public BallProjectile() {
			_Init();
		}
	}
}
