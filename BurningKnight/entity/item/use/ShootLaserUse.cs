using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class ShootLaserUse : ShootUse {
		private Laser laser;

		public override void Update(Entity entity, Item item, float dt) {
			base.Update(entity, item, dt);

			if (laser != null) {
				if (laser.Done) {
					laser = null;

					return;
				}
				
				var aim = entity.GetComponent<AimComponent>();
				var from = aim.Center;
				var am = aim.RealAim;
				var a = MathUtils.Angle(am.X - from.X, am.Y - from.Y);

				laser.BodyComponent.Body.Rotation = a;
				laser.Position = from;
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			SpawnProjectile = (entity, item) => {
				laser = new Laser();
				var aim = entity.GetComponent<AimComponent>();
				var from = aim.Center;
				var am = aim.RealAim;
				var a = MathUtils.Angle(am.X - from.X, am.Y - from.Y);
				
				laser.Damage = 1;
				laser.StarterOwner = entity;
				laser.Owner = entity;
				
				entity.Area.Add(laser);
				laser.BodyComponent.Body.Rotation = a;
				laser.Position = from;
				laser.Color = ProjectileColor.Red;
				laser.Recalculate();
			};
		}
	}
}