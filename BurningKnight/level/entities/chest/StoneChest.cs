using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class StoneChest : Chest {
		public override string GetSprite() {
			return "stone_chest";
		}
		
		public override void AddComponents() {
			base.AddComponents();

			CanOpen = false;
			AddComponent(new ExplodableComponent());
		}

		public override string GetPool() {
			return "bk:stone_chest";
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent ex) {
				Open(ex.Who);
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}