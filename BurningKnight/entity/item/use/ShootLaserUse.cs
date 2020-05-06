using BurningKnight.entity.projectile;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ShootLaserUse : ShootUse {
		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			SpawnProjectile = (entity, item) => {
				var laser = new Laser();

				laser.Damage = 1;
				laser.StarterOwner = entity;
				laser.Owner = entity;
				
				entity.Area.Add(laser);
				laser.Recalculate();
			};
		}
	}
}