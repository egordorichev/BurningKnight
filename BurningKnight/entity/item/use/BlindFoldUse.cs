using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class BlindFoldUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (entity.TryGetComponent<WeaponComponent>(out var w)) {
				w.Disabled = true;
			}
			
			if (entity.TryGetComponent<ActiveWeaponComponent>(out var aw)) {
				aw.Disabled = true;
			}
		}
	}
}