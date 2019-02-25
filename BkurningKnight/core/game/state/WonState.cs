using BurningKnight.core;
using BurningKnight.core.util;

namespace BurningKnight.core.game.state {
	public class WonState : State {
		public override Void Init() {
			base.Init();
			Dungeon.Dark = 0;
			Dungeon.DarkR = Dungeon.MAX_R;
			Player.Instance.Done = true;
			Player.Instance.Remove();
			Player.Instance = null;
			Tween.To(new Tween.Task(1, 0.3f) {
				public override float GetValue() {
					return 0;
				}

				public override Void SetValue(float Value) {
					Dungeon.Dark = Value;
				}
			});
		}

		public override Void Render() {
			base.Render();
			RenderPortal();

			if (Input.Instance.WasPressed("X") && Dungeon.Dark == 1) {
				Tween.To(new Tween.Task(0, 0.3f) {
					public override float GetValue() {
						return 1;
					}

					public override Void SetValue(float Value) {
						Dungeon.Dark = Value;
					}

					public override Void OnEnd() {
						MainMenuState.VoidMusic.Stop();
						Dungeon.NewGame(false, -2);
					}
				});
			} 
		}

		public override Void RenderUi() {
			base.RenderUi();
			float S = 14;
			Graphics.PrintCenter("You won!", Graphics.Medium, 0, Display.UI_HEIGHT / 2 + S * 2);
			Graphics.PrintCenter("(kind-of, that's only small part of the game)", Graphics.Medium, 0, Display.UI_HEIGHT / 2 + S);
			Graphics.PrintCenter("Thanks so much for playing!", Graphics.Medium, 0, Display.UI_HEIGHT / 2 - S);
			Graphics.PrintCenter("This was an alpha build of the game,", Graphics.Medium, 0, Display.UI_HEIGHT / 2 - S * 2);
			Graphics.PrintCenter("so much more stuff will be added", Graphics.Medium, 0, Display.UI_HEIGHT / 2 - S * 3);
			Graphics.PrintCenter("and fixed. Please come back later :)", Graphics.Medium, 0, Display.UI_HEIGHT / 2 - S * 4);
			Graphics.PrintCenter("Press X", Graphics.Medium, 0, Display.UI_HEIGHT / 2 - S * 6);
		}
	}
}
