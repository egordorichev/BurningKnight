using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 18;
			
			AddComponent(new AnimationComponent("old_man"));
		}
	}
}