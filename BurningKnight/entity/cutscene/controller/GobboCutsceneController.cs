using BurningKnight.entity.cutscene.entity;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.cutscene.controller {
	public class GobboCutsceneController : CutsceneController {
		private BabyGobbo baby;
		private OldGobbo dad;
		
		public override void PostInit() {
			base.PostInit();

			baby = Area.Find<BabyGobbo>();
			dad = Area.Find<OldGobbo>();
			
			dad.GetComponent<DialogComponent>().Start("dad_0");
		}
	}
}