using System;
using BurningKnight.entity.projectile;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ShootUse : ItemUse {
		public Action<Entity, Item> SpawnProjectile;

		public ShootUse(Action<Entity, Item> spawn) {
			SpawnProjectile = spawn;
		}
		
		public void Use(Entity entity, Item item) {
			SpawnProjectile(entity, item);
		}
	}
}