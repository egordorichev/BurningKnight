using System;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.entity.item.use {
	public class ShootUse : ItemUse {
		public Action<Entity, Item> SpawnProjectile;

		public override void Use(Entity entity, Item item) {
			SpawnProjectile(entity, item);
			Camera.Instance.ShakeMax(3);
		}
	}
}