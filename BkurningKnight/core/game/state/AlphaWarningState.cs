using BurningKnight.core.game.input;
using BurningKnight.core.util;

namespace BurningKnight.core.game.state {
	public class AlphaWarningState : State {
		private float Alpha = 0;
		private bool Did;

		public override Void Init() {
			base.Init();
			Dungeon.SetBackground(new Color(0, 0, 0, 1));
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Alpha += ((this.Did ? 0 : 1) - this.Alpha) * Dt * 5;

			if (!Did && (Input.Instance.WasPressed("X"))) {
				Did = true;
				Color Color = Color.WHITE;
				float T = 0.1f;
				Tween.To(new Tween.Task(Color.R, T) {
					public override float GetValue() {
						return 0;
					}

					public override Void SetValue(float Value) {
						Dungeon.GetBackground2().R = Value;
					}

					public override Void OnEnd() {
						Color Color = Color.ValueOf("#1a1932");
						float T = 0.2f;
						Tween.To(new Tween.Task(Color.R, T) {
							public override float GetValue() {
								return Dungeon.GetBackground2().R;
							}

							public override Void SetValue(float Value) {
								Dungeon.GetBackground2().R = Value;
							}

							public override Void OnEnd() {
								Dungeon.Game.SetState(new MainMenuState());
							}
						});
						Tween.To(new Tween.Task(Color.G, T) {
							public override float GetValue() {
								return Dungeon.GetBackground2().G;
							}

							public override Void SetValue(float Value) {
								Dungeon.GetBackground2().G = Value;
							}
						});
						Tween.To(new Tween.Task(Color.B, T) {
							public override float GetValue() {
								return Dungeon.GetBackground2().B;
							}

							public override Void SetValue(float Value) {
								Dungeon.GetBackground2().B = Value;
							}
						});
					}
				}).Delay(0.5f);
				Tween.To(new Tween.Task(Color.G, T) {
					public override float GetValue() {
						return 0;
					}

					public override Void SetValue(float Value) {
						Dungeon.GetBackground2().G = Value;
					}
				}).Delay(0.5f);
				Tween.To(new Tween.Task(Color.B, T) {
					public override float GetValue() {
						return 0;
					}

					public override Void SetValue(float Value) {
						Dungeon.GetBackground2().B = Value;
					}
				}).Delay(0.5f);
			} 
		}

		public override Void RenderUi() {
			base.RenderUi();
			float V = Did ? 0.7f : 1f;
			Graphics.Small.SetColor(V, V, V, this.Alpha);
			int Y = Display.UI_HEIGHT / 3 * 2;
			Graphics.Print("Warning", Graphics.Small, Y);
			Graphics.Print("This game is still in alpha", Graphics.Small, Y - 32);
			Graphics.Print("This means, that it still might have bugs in it", Graphics.Small, Y - 48);
			Graphics.Print("If you meet any, please report them", Graphics.Small, Y - 64);
			Graphics.Print("Press X to continue", Graphics.Small, Y - 96);
			Graphics.Small.SetColor(1, 1, 1, 1);
		}
	}
}
