using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiInput : UiEntity, InputProcessor {
		private string Input = "";
		private string Placeholder = "";
		private bool Open = false;
		public int W = 64;
		public int H = 12;
		private int Pw = 0;

		public string GetInput() {
			return this.Input;
		}

		public override Void Init() {
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.AddProcessor(this);
			this.CalcPW();
		}

		private Void CalcPW() {
			Graphics.Layout.SetText(Graphics.Medium, this.Placeholder);
			this.Pw = (int) Graphics.Layout.Width;
			this.W = this.Pw;
			this.X = (Display.GAME_WIDTH - this.Pw) / 2;
		}

		private Void CalcW() {
			Graphics.Layout.SetText(Graphics.Medium, this.Input);
			this.W = (int) Graphics.Layout.Width;
			this.X = (Display.GAME_WIDTH - this.W) / 2;
		}

		public Void SetPlaceholder(string Placeholder) {
			this.Placeholder = Placeholder;
		}

		public override Void Render() {
			string Str = this.Input;

			if (!this.Open && Str.IsEmpty()) {
				Str = this.Placeholder;
				Graphics.Medium.SetColor(0.7f, 0.7f, 0.7f, 1f);
			} 

			Graphics.Medium.Draw(Graphics.Batch, this.Open ? Str + "|" : Str, this.X, this.Y + 12);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}

		public Void OnEnter(string Input) {

		}

		public Void SetInput(string Input) {
			this.Input = Input;
		}

		public override bool KeyDown(int Keycode) {
			if (Keycode == Input.Keys.ENTER && this.Open) {
				this.OnEnter(this.Input);
				this.Open = false;

				if (this.Input.IsEmpty()) {
					this.CalcPW();
				} else {
					this.CalcW();
				}

			} else if (Keycode == Input.Keys.BACKSPACE && this.Open && this.Input.Length() > 0) {
				this.Input = this.Input.Substring(0, this.Input.Length() - 1);
				this.CalcW();
			} 

			return false;
		}

		public override bool KeyUp(int Keycode) {
			return false;
		}

		public override Void Destroy() {
			base.Destroy();
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.RemoveProcessor(this);
		}

		public override bool KeyTyped(char Character) {
			if (this.Open && IsPrintableChar(Character)) {
				this.Input += Character;
				this.CalcW();
			} 

			return false;
		}

		public override bool TouchDown(int ScreenX, int ScreenY, int Pointer, int Button) {
			bool Was = this.Open;
			Vector2 Mouse = Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.UiMouse;
			this.Open = (CollisionHelper.Check((int) Mouse.X, (int) Mouse.Y, (int) this.X, (int) this.Y, this.W, this.H));

			if (Was && !this.Open) {
				if (this.Input.IsEmpty()) {
					this.CalcPW();
				} else {
					this.CalcW();
				}

			} else if (!Was && this.Open) {
				this.CalcW();
			} 

			return this.Open;
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
