using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiInput : UiEntity, InputProcessor {
		public int H = 12;
		private string Input = "";
		private bool Open;
		private string Placeholder = "";
		private int Pw;
		public int W = 64;

		public string GetInput() {
			return Input;
		}

		public override void Init() {
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.AddProcessor(this);
			CalcPW();
		}

		private void CalcPW() {
			Graphics.Layout.SetText(Graphics.Medium, Placeholder);
			Pw = (int) Graphics.Layout.Width;
			W = Pw;
			this.X = (Display.Width - Pw) / 2;
		}

		private void CalcW() {
			Graphics.Layout.SetText(Graphics.Medium, Input);
			W = (int) Graphics.Layout.Width;
			this.X = (Display.Width - W) / 2;
		}

		public void SetPlaceholder(string Placeholder) {
			this.Placeholder = Placeholder;
		}

		public override void Render() {
			var Str = Input;

			if (!Open && Str.IsEmpty()) {
				Str = Placeholder;
				Graphics.Medium.SetColor(0.7f, 0.7f, 0.7f, 1f);
			}

			Graphics.Medium.Draw(Graphics.Batch, Open ? Str + "|" : Str, this.X, this.Y + 12);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}

		public void OnEnter(string Input) {
		}

		public void SetInput(string Input) {
			this.Input = Input;
		}

		public override bool KeyDown(int Keycode) {
			if (Keycode == Input.Keys.ENTER && Open) {
				OnEnter(Input);
				Open = false;

				if (Input.IsEmpty())
					CalcPW();
				else
					CalcW();
			}
			else if (Keycode == Input.Keys.BACKSPACE && Open && Input.Length() > 0) {
				Input = Input.Substring(0, Input.Length() - 1);
				CalcW();
			}

			return false;
		}

		public override bool KeyUp(int Keycode) {
			return false;
		}

		public override void Destroy() {
			base.Destroy();
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.RemoveProcessor(this);
		}

		public override bool KeyTyped(char Character) {
			if (Open && IsPrintableChar(Character)) {
				Input += Character;
				CalcW();
			}

			return false;
		}

		public override bool TouchDown(int ScreenX, int ScreenY, int Pointer, int Button) {
			var Was = Open;
			Vector2 Mouse = Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.UiMouse;
			Open = CollisionHelper.Check((int) Mouse.X, (int) Mouse.Y, (int) this.X, (int) this.Y, W, H);

			if (Was && !Open) {
				if (Input.IsEmpty())
					CalcPW();
				else
					CalcW();
			}
			else if (!Was && Open) {
				CalcW();
			}

			return Open;
		}

		public override bool TouchUp(int ScreenX, int ScreenY, int Pointer, int Button) {
			return false;
		}

		public override bool TouchDragged(int ScreenX, int ScreenY, int Pointer) {
			return false;
		}

		public override bool MouseMoved(int ScreenX, int ScreenY) {
			return false;
		}

		public override bool Scrolled(int Amount) {
			return false;
		}

		public static bool IsPrintableChar(char C) {
			return !Character.IsISOControl(C);
		}
	}
}