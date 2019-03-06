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
		}
		
		public void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			base.HandleEvent(e);

			if (e is HealthModifiedEvent ev) {
				if (HasNoHealth()) {
					Kill(ev.From);
				}
			} else if (e is DiedEvent) {
				Done = true;
			}

			return false;
		}

		protected virtual bool HasNoHealth() {
			return GetComponent<HealthComponent>().Health == 0;
		}
	}
}