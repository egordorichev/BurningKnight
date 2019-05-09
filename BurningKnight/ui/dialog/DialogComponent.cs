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
	public class DialogComponent : Component {
		public UiDialog Dialog;
		private Dialog currentData;

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
					var next = currentData.DecideNext();

					if (next == null) {
						Input.Blocked = 0;
						return true;
					}

					Start(next);
					return false;
				};
			}
			
			Input.Blocked = 1;
			
			currentData = dialog;
			Dialog.Say(dialog.Modify(Locale.Get(id)));
			Dialog.Str.Renderer = RenderChoice;
		}

		private void RenderChoice(Vector2 pos, int i) {
			if (currentData is ChoiceDialog c && i == c.Choice) {
				Graphics.Print(">", Font.Small, pos);
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Dialog != null && Dialog.DoneSaying && currentData is ChoiceDialog c) {
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