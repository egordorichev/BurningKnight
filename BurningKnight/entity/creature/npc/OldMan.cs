using BurningKnight.entity.component;
using BurningKnight.util.dialog;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AnimationComponent("old_man"));

			// new Dialog("its_dangerous_to_go_alone", "shut_up");
		}
	}
}