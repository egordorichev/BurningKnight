using BurningKnight.entity.component;
using BurningKnight.entity.cutscene.entity;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.graphics.gamerenderer;
using Lens.util.camera;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.cutscene.controller {
	public class GobboCutsceneController : CutsceneController {
		private BabyGobbo baby;
		private OldGobbo dad;
		private Gobbo gobbo;
		private Vector2 dadStart;
		
		public override void PostInit() {
			base.PostInit();

			baby = Area.Find<BabyGobbo>();
			dad = Area.Find<OldGobbo>();
			dadStart = dad.BottomCenter;

			dad.GraphicsComponent.Flipped = true;

			Timer.Add(() => FirstPart(), 1);
			PixelPerfectGameRenderer.GameScale = 2;
		}

		public override void Destroy() {
			base.Destroy();
			PixelPerfectGameRenderer.GameScale = 1;
		}

		private void FirstPart() {
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

						Timer.Add(() => {
							State.Say("20 years later", () => {
								dad.Done = true;
								gobbo = new Gobbo();
								Area.Add(gobbo);
								gobbo.BottomCenter = dadStart;
								gobbo.GraphicsComponent.Flipped = true;

								Timer.Add(() => {
									SecondPart();
								}, 1f);
							});
						}, 2f);
					});
				});
			});
		}

		private void SecondPart() {
			var gobboDialog = gobbo.GetComponent<DialogComponent>();
			var sonDialog = baby.GetComponent<DialogComponent>();

			gobboDialog.Dialog.AlwaysShowArrow = true;

			Start(gobboDialog, "gobbo_0", () => {
				gobboDialog.Close();

				Start(sonDialog, "son_0", () => {
					sonDialog.Close();

					Start(gobboDialog, "gobbo_1", () => {
						gobboDialog.Close();
						gobbo.GraphicsComponent.Flipped = false;
						gobbo.GetComponent<AnimationComponent>().Animation.Tag = "run";
						gobbo.RunAway = true;

						Timer.Add(() => {
							State.Transition(() => {
								Run.Depth = -2;
							});
						}, 2f);
					});
				});
			});
		}
	}
}