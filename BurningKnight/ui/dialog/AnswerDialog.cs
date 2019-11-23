using Lens.input;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.dialog {
	public class AnswerDialog : Dialog {
		// Keep in sync with AnswerType!
		public static string[] Types = {
			"Text", "Seed"
		};
		
		public string Answer = "";
		public bool Focused = true;
		public AnswerType Type;
		
		public AnswerDialog(string id, AnswerType type, string[] next = null) : base(id, next) {
			Type = type;
		}

		public override void Reset() {
			base.Reset();

			Focused = true;
			Answer = "";
		}

		public override string Modify(string dialog) {
			return $"{dialog}\n[rn 0] ";
		}

		public void HandleInput(TextInputEventArgs args) {
			if (args.Key == Keys.Back) {
				if (Answer.Length > 0) {
					Answer = Answer.Substring(0, Answer.Length - 1);
				}
			} else if (args.Key == Keys.Enter) {
				Focused = false;
			} else {
				var c = CheckChar(args.Character);
				
				if (c != '\0') {
					Answer += c;	
				}
			}
		}

		private char CheckChar(char c) {
			switch (Type) {
				case AnswerType.Text: return c;

				case AnswerType.Seed: {
					if (char.IsLower(c)) {
						c = char.ToUpper(c);
					}
					
					return Answer.Length < 8 && Rnd.SeedChars.IndexOf(c) != -1 && c != '_' ? c : '\0';
				}
			}

			return '\0';
		}

		private void AttachLetter(char c) {
			if (!Focused) {
				return;
			}
			
			Answer += c;

			if (Answer.Length == 8) {
				Focused = false;
			}
		}
		
		public void CheckGamepadInput(GamepadData data) {
			if (data.WasPressed(Buttons.X)) {
				AttachLetter('X');
			}
			
			if (data.WasPressed(Buttons.Y)) {
				AttachLetter('Y');
			}
			
			if (data.WasPressed(Buttons.A)) {
				AttachLetter('A');
			}
			
			if (data.WasPressed(Buttons.B)) {
				AttachLetter('B');
			}
			
			if (data.WasPressed(Buttons.DPadUp)) {
				AttachLetter('U');
			}
			
			if (data.WasPressed(Buttons.DPadDown)) {
				AttachLetter('D');
			}
			
			if (data.WasPressed(Buttons.DPadLeft)) {
				AttachLetter('L');
			}
			
			if (data.WasPressed(Buttons.DPadRight)) {
				AttachLetter('R');
			}
		}
	}
}