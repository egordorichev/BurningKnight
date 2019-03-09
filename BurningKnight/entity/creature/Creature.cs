using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.level;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new HealthComponent());
			AddComponent(new StateComponent());
			AddComponent(new RoomComponent());
			AddComponent(new BuffsComponent());
		}
		
		public void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if (HasNoHealth(ev)) {
					Kill(ev.From);
				}
			} else if (e is DiedEvent) {
				Done = true;
			}

			return base.HandleEvent(e);
		}

		protected virtual bool HasNoHealth(HealthModifiedEvent e) {
			return GetComponent<HealthComponent>().Health == -e.Amount;
		}
	}
}