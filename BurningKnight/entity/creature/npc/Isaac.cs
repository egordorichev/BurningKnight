using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class Isaac : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 11;
			
			AddComponent(new AnimationComponent("isaac"));
			AddComponent(new CloseDialogComponent("isaac_0", "isaac_1", "isaac_2"));
		}
	}
}