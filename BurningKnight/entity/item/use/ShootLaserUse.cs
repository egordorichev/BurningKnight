using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class ShootLaserUse : ShootUse {
		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			SpawnProjectile = (entity, item) => {
				var laser = new Laser();
				var aim = entity.GetComponent<AimComponent>();
				var from = aim.Center;
				var am = aim.RealAim;
				var a = MathUtils.Angle(am.X - from.X, am.Y - from.Y);
				
				laser.Damage = 1;
				laser.StarterOwner = entity;
				laser.Owner = entity;
				
				entity.Area.Add(laser);
				laser.BodyComponent.Body.Rotation = a;
				laser.Center = from;
				laser.Color = ProjectileColor.Red;
				laser.Recalculate();
			};
		}
	}
}