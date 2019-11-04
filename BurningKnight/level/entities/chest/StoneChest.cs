using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class StoneChest : Chest {
		public override void AddComponents() {
			base.AddComponents();
			
			RemoveComponent<InteractableComponent>();
			AddComponent(new ExplodableComponent());
		}

		protected override void DefineDrops() {
			var drops = GetComponent<DropsComponent>();
			
			drops.Add(new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo")	
			));
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent) {
				Open();
			}
			
			return base.HandleEvent(e);
		}
	}
}