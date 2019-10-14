using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class Elon : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 17;
			Height = 19;
			
			AddComponent(new AnimationComponent("elon"));
			AddComponent(new CloseDialogComponent("elon_0"));
		}
	}
}