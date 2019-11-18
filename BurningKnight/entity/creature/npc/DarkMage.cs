using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class DarkMage : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 14;
			Height = 17;
			
			AddComponent(new AnimationComponent("dark_mage"));
			AddComponent(new CloseDialogComponent("dm_0"));
		}
	}
}