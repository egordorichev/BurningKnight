using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class Maanex : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 15;
			
			AddComponent(new AnimationComponent("maanex"));
			AddComponent(new CloseDialogComponent("maanex_0", "maanex_1", "maanex_2", "maanex_3", "maanex_4"));
		}
	}
}