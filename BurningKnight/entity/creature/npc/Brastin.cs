using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class Brastin : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 13;
			Height = 18;
			
			AddComponent(new AnimationComponent("brastin"));
			AddComponent(new CloseDialogComponent("brastin_0"));
		}
	}
}