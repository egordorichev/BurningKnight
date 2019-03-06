using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens.entity.component.graphics;
using Lens.graphics;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		public void Render() {
			if (Item == null) {
				return;
			}
			
			Graphics.Render(Item.GetComponent<SliceComponent>().Sprite, Entity.Position);
		}
		
		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Normal;
		}
	}
}