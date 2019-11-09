using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class StoneChest : Chest {
		public StoneChest() {
			Sprite = "stone_chest";
		}
		
		public override void AddComponents() {
			base.AddComponents();

			CanOpen = false;
			AddComponent(new ExplodableComponent());
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:red_chest");
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent) {
				Open();
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}