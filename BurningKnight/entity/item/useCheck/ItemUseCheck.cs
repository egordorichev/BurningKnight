using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.useCheck {
	public class ItemUseCheck {
		public bool CanUse(Entity entity, Item item) {
			return item.Delay < 0.05f && (item.Type != ItemType.Artifact || !entity.GetComponent<BuffsComponent>().Has<CharmedBuff>());
		}
	}
}