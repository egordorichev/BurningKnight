using BurningKnight.core.assets;

namespace BurningKnight.core.ui {
	public class UiTextInput : UiButton, InputProcessor {
		private string Lb;
		private bool Active;
		public string Input = "";

		public UiTextInput(string Label, int X, int Y) {
			base(Label, X, Y);
			Lb = this.Label;
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.AddProcessor(this);
		}

		public override Void Destroy() {
			base.Destroy();
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.RemoveProcessor(this);
		}

		public override Void OnClick() {
			base.OnClick();
			Active = !Active;

			if (Active) {
				SetLabel(Lb + " " + Input + "_");
			} else {
				SetLabel(Lb + " " + Input);
			}

		}

		public override Void Unselect() {
			base.Unselect();
			Active = false;
			SetLabel(Lb + " " + Input);
		}

		public override Void Select() {
			base.Select();
			Active = true;
			SetLabel(Lb + " " + Input + "_");
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.WasPressed("pause")) {
				OnBackSpace();
			} 
		}

		private Void OnEnter() {
			Active = false;
			SetLabel(Lb + " " + Input);
			Audio.PlaySfx("menu/select");
		}

		private Void OnBackSpace() {
			this.Input = this.Input.Substring(0, this.Input.Length() - 1);
			SetLabel(Lb + " " + Input + "_");
			Audio.PlaySfx("menu/moving");
		}

		public override bool KeyDown(int Keycode) {
			if (Active) {
				if (Keycode == Input.Keys.BACKSPACE && Input.Length() > 0) {
					OnBackSpace();
				} else if (Keycode == Input.Keys.V && Gdx.Input.IsKeyPressed(Input.Keys.CONTROL_LEFT)) {
					int Len = GetMaxLength();

					if (Len != -1 && Input.Length() >= Len) {
						return false;
					} 

					string Str = Gdx.App.GetClipboard().GetContents();

					for (int I = 0; I < Str.Length(); I++) {
						if (Validate(Str.CharAt(I)) == '\0') {
							return false;
						} 
					}

					if (Len != -1 && Str.Length() + Input.Length() > Len) {
						Str = Str.Substring(0, Len - (Str.Length() + Input.Length()));
					} 

					Input += Str;
					SetLabel(Lb + " " + Input + "_");
					Audio.PlaySfx("menu/moving");
				} 
			} 

			return false;
		}

		public override bool KeyUp(int Keycode) {
			return false;
		}

		public char Validate(char Ch) {
			return UiInput.IsPrintableChar(Ch) ? Ch : '\0';
		}

		public override bool KeyTyped(char Character) {
			if (Active && Validate(Character) != '\0') {
				int Len = GetMaxLength();

				if (Len != -1 && Input.Length() >= Len) {
					return false;
				} 

				Input += Validate(Character);
				SetLabel(Lb + " " + Input + "_");
				Audio.PlaySfx("menu/moving");
			} 

			return false;
		}

		public override bool TouchDown(int ScreenX, int ScreenY, int Pointer, int Button) {
			return false;
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

		public int GetMaxLength() {
			return -1;
		}
	}
}
