using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class MaPuzzle : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 8;
			
			AddComponent(new AnimationComponent("mapuzzle"));
			AddComponent(new CloseDialogComponent("mapuzzle_0"));
			
			GetComponent<DialogComponent>().Dialog.Voice = 17;
		}
	}
}