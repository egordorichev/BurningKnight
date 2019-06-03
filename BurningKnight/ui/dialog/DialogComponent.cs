using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.dialog {
	public delegate void DialogCallback(DialogComponent d);
	
	public class DialogComponent : Component {
		public UiDialog Dialog;
		public Dialog Last;
		public Dialog Current;

		public DialogCallback OnNext;
		public Entity To;

		private bool added;

		private void HandleInput(object sender, TextInputEventArgs args) {
			if (Current is AnswerDialog a) {
				if (a.Focused) {
					if (args.Key == Keys.Back) {
						if (a.Answer.Length > 0) {
							a.Answer = a.Answer.Substring(0, a.Answer.Length - 1);
						}
					} else if (args.Key == Keys.Enter) {
						var s = a.Answer;
						
						a.Focused = false;
						a.Answer = "";
					} else {
						a.Answer += args.Character;	
					}
				}
			}
		}

		public override void Init() {
			base.Init();
			
			Engine.Instance.Window.TextInput += HandleInput;

			Dialog = new UiDialog();
			Dialog.Owner = Entity;
			
			Dialog.OnEnd += () => {
				foreach (var c in Current.Callbacks) {
					c(Current);
				}
				
				var next = Current.GetNext();

				if (next == null) {
					Last = Current;
					Current = null;
					OnNext?.Invoke(this);

					if (To != null) {
						var input = To.GetComponent<PlayerInputComponent>();

						input.InDialog = false;
						input.Dialog = null;
							
						To = null;
					}
						
					return true;
				}

				Setup(next);
				OnNext?.Invoke(this);
				return false;
			};
		}

		public override void Destroy() {
			base.Destroy();
			Engine.Instance.Window.TextInput -= HandleInput;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (added) {
				return;
			}

			added = true;
			Engine.Instance.State.Ui.Add(Dialog);
		}

		public void Start(string id, Entity to = null) {
			var dialog = Dialogs.Get(id);

			if (dialog == null) {
				return;
			}

			Setup(dialog, to);
		}

		public void Close() {
			if (Dialog == null || Current == null) {
				return;
			}

			Dialog.Close();
			Last = Current;
			Current = null;
		}
		
		private void Setup(Dialog dialog, Entity to = null) {
			Last = Current;
			Current = dialog;

			var c = Locale.Get(dialog.Id);
			var s = dialog.Modify(c);
			
			Dialog.Say(s);
			Dialog.Str.Renderer = RenderChoice;
			
			if (to is Player p) {
				var input = p.GetComponent<PlayerInputComponent>();

				input.InDialog = true;
				input.Dialog = this;

				To = to;
			}
		}

		private void RenderChoice(Vector2 pos, int i) {
			if (Current is ChoiceDialog c) {
				if (i == c.Choice) {
					Graphics.Print(">", Font.Small, pos);
				}
			} else if (Current is AnswerDialog a) {
				Graphics.Print($"{a.Answer}{(a.Focused && Engine.Time % 0.8f > 0.4f ? "|" : "")}", Font.Small, pos);
			}
		}
	}
}