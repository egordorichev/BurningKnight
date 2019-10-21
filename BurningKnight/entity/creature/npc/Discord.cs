using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class Discord : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 12;
			
			AddComponent(new AnimationComponent("discord"));
			AddComponent(new CloseDialogComponent("discord_0"));
		}
	}
}