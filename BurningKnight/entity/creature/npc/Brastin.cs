using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class Brastin : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 13;
			Height = 18;
			
			AddComponent(new AnimationComponent("brastin"));
			AddComponent(new CloseDialogComponent("brastin_0"));
			GetComponent<DialogComponent>().Dialog.Voice = 3;
		}
	}
}