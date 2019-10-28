using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class GiveLaserAimUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			entity.GetComponent<AimComponent>().ShowLaserLine = true;
		}
	}
}