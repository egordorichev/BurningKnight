using BurningKnight.entity.component;
using BurningKnight.entity.cutscene.entity;
using BurningKnight.ui.dialog;
using Lens.util.timer;

namespace BurningKnight.entity.cutscene.controller {
	public class GobboCutsceneController : CutsceneController {
		private BabyGobbo baby;
		private OldGobbo dad;
		
		public override void PostInit() {
			base.PostInit();

			baby = Area.Find<BabyGobbo>();
			dad = Area.Find<OldGobbo>();

			dad.GraphicsComponent.Flipped = true;

			var dadDialog = dad.GetComponent<DialogComponent>();
			var sonDialog = baby.GetComponent<DialogComponent>();

			dadDialog.Dialog.AlwaysShowArrow = true;
			sonDialog.Dialog.AlwaysShowArrow = true;

			Start(dadDialog, "dad_0", () => {
				dadDialog.Close();

				Start(sonDialog, "son_0", () => {
					sonDialog.Close();

					Start(dadDialog, "dad_1", () => {
						dadDialog.Close();
						dad.GraphicsComponent.Flipped = false;
						dad.GetComponent<AnimationComponent>().Animation.Tag = "run";
						dad.RunAway = true;
					});
				});
			});
		}
	}
}