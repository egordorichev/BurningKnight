using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveItemComponent : ItemComponent {
		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Input.WasPressed(Controls.Active)) {
				Item.Use((Player) Entity);
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Active;
		}
	}
}