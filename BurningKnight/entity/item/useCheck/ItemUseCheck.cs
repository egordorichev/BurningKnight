using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity.item.useCheck {
	public class ItemUseCheck {
		public virtual bool CanUse(Entity entity, Item item) {
			return item.Delay < 0.05f && (item.Type != ItemType.Weapon || !entity.GetComponent<BuffsComponent>().Has<CharmedBuff>());
		}
	}
}