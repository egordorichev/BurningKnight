using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		protected bool AtBack = true;
		
		public void Render() {
			if (Item != null && Item.Renderer != null) {
				var sh = Shaders.Item;
				Shaders.Begin(sh);
				sh.Parameters["time"].SetValue(Engine.Time * 0.1f);
				sh.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);

				Item.Renderer.Render(AtBack, Engine.Instance.State.Paused, Engine.Delta);

				Shaders.End();
			}
		}
		
		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Weapon;
		}
	}
}