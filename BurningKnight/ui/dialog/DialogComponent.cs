using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
using Lens.entity.component;
using Lens.graphics;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.dialog {
	public delegate void DialogCallback();
	
	public class DialogComponent : Component {
		public UiDialog Dialog;
		public Dialog Last;
		public Dialog Current;

		public DialogCallback OnNext;

		public void Start(string id) {
			var dialog = Dialogs.Get(id);

			if (dialog == null) {
				return;
			}
			
			if (Dialog == null) {
				Dialog = new UiDialog();
				Engine.Instance.State.Ui.Add(Dialog);

				Dialog.Owner = Entity;
				Dialog.OnEnd = () => {
					var next = Current.GetNext();

					if (next == null) {
						Last = Current;
						Current = null;
						OnNext?.Invoke();
						return true;
					}

					Setup(next);
					OnNext?.Invoke();
					return false;
				};
			}

			Setup(dialog);
		}
		
		private void Setup(Dialog dialog) {
			Last = Current;
			Current = dialog;
			Dialog.Say(dialog.Modify(Locale.Get(dialog.Id)));
			Dialog.Str.Renderer = RenderChoice;
		}

		private void RenderChoice(Vector2 pos, int i) {
			if (Current is ChoiceDialog c && i == c.Choice) {
				Graphics.Print(">", Font.Small, pos);
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Dialog != null && Dialog.DoneSaying && Current is ChoiceDialog c) {
				var data = LocalPlayer.Locate(Engine.Instance.State.Area).GetComponent<GamepadComponent>().Controller;
				
				if (Input.WasPressed(Controls.Up, data, true)) {
					c.Choice -= 1;

					if (c.Choice < 0) {
						c.Choice += c.Options.Length;
					}
				}
				
				if (Input.WasPressed(Controls.Down, data, true)) {
					c.Choice = (c.Choice + 1) % c.Options.Length;
				}
			}
		}
	}
}