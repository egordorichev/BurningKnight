using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.cutscene.entity {
	public class BabyGobbo : CutsceneEntity {
		public override void AddComponents() {
			base.AddComponents();

			Width = 8;
			Height = 8;
			
			AddComponent(new AnimationComponent("baby_gobbo"));
			GetComponent<DialogComponent>().Dialog.Voice = 7;
		}
	}
}