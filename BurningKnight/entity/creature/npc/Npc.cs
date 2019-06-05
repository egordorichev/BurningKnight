using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class Npc : Creature {
		public const int Padding = 4;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new DialogComponent());
			GetComponent<HealthComponent>().Unhittable = true;
		}
	}
}