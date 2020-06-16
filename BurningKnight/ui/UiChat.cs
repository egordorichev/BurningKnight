using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui {
	public class UiChat : Entity {
		private bool open;
		private string input = "";

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = 32;

			// Engine.Instance.Window.TextInput += HandleInput;
		}

		public override void Destroy() {
			base.Destroy();
			// Engine.Instance.Window.TextInput -= HandleInput;
		}
		
		public void HandleInput(object e, TextInputEventArgs args) {
			if (Engine.Instance.State.Paused) {
				return;
			}
			
			if (args.Key == Keys.Back) {
				if (open && input.Length > 0) {
					Audio.PlaySfx("ui_moving");
					input = input.Substring(0, input.Length - 1);
				}
			} else if (args.Key == Keys.Enter) {
				if (open) {
					open = false;
					Input.Blocked = 0;
					Audio.PlaySfx("ui_exit");

					if (input != "") {
						if (input == "muffin") {
							input = "##[cl red]NANI??!?!?";
						}
						
						var player = LocalPlayer.Locate(Engine.Instance.State.Area);

						if (player != null) {
							player.GetComponent<DialogComponent>().StartAndClose(input, 5);
							input = "";
						}
					}
				}
			} else {
				var c = args.Character;

				if (c == '/') {
					open = !open;

					if (open) {
						Input.Blocked = 1;
						Audio.PlaySfx("ui_select");
					} else {
						Input.Blocked = 0;
						Audio.PlaySfx("ui_exit");
					}
				} else if (open && c != '\0' && c != '\t' && c != '\n') {
					input += c;
					Audio.PlaySfx("ui_moving");
				}
			}
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void Render() {
			if (!open) {
				return;
			}

			Graphics.Print($"> {input}{(t % 1f > 0.5f ? "" : "|")}", Font.Small, new Vector2(4, Display.UiHeight - 16));
		}
	}
}