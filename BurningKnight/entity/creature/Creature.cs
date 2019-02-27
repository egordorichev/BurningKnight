using BurningKnight.entity.component;
using BurningKnight.entity.level;
using BurningKnight.save;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity {
		protected override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new HealthComponent());
			AddComponent(new StateComponent<Creature>());
		}
		
		public void Kill() {
			GetComponent<HealthComponent>().Dead = true;
		}
	}
}