using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.dialog {
	public delegate void DialogCallback(DialogComponent d);
	
	public class DialogComponent : Component {
		public UiDialog Dialog;
		public Dialog Last;
		public Dialog Current;

		public DialogCallback OnNext;
		
		public void Start(string id, Entity to = null) {
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
						OnNext?.Invoke(this);
						return true;
					}

					Setup(next);
					OnNext?.Invoke(this);
					return false;
				};
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
			Dialog.Say(dialog.Modify(Locale.Get(dialog.Id)));
			Dialog.Str.Renderer = RenderChoice;

			if (to is Player p) {
				var input = p.GetComponent<PlayerInputComponent>();

				input.InDialog = true;
				input.Dialog = this;
			}
		}

		private void RenderChoice(Vector2 pos, int i) {
			if (Current is ChoiceDialog c && i == c.Choice) {
				Graphics.Print(">", Font.Small, pos);
			}
		}
	}
}