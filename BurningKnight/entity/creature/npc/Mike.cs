using BurningKnight.entity.component;
using BurningKnight.save;

namespace BurningKnight.entity.creature.npc {
	public class Mike : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 11;
			Height = 13;
			
			AddComponent(new AnimationComponent("mike"));
			// AddComponent(new CloseDialogComponent("elon_0"));
			// GetComponent<DialogComponent>().Dialog.Voice = 15;
		}
	}
}