using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class Milt : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 8;
			
			AddComponent(new AnimationComponent("milt"));
			AddComponent(new CloseDialogComponent("milt_1"));
			
			GetComponent<DialogComponent>().Dialog.Voice = 21;
		}
	}
}