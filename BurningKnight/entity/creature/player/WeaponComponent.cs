using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens;
using Lens.entity.component.graphics;
using Lens.graphics;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		public virtual void Render() {
			Item?.Renderer?.Render(true, Engine.Instance.State.Paused, Engine.Delta);
		}
		
		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Normal;
		}
	}
}