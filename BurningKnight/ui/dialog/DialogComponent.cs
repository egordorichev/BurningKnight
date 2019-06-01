using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.dialog {
	public delegate void DialogCallback(DialogComponent d);
	
	public class DialogComponent : Component {
		public UiDialog Dialog;
		public Dialog Last;
		public Dialog Current;

		public DialogCallback OnNext;
		public Entity To;

		private bool added;

		public override void Init() {
			base.Init();
			
			Dialog = new UiDialog();
			Dialog.Owner = Entity;
			
			Dialog.OnEnd += () => {
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
			Dialog.Say(dialog.Modify(Locale.Get(dialog.Id)));
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
				Graphics.Print($"{a.Answer}{(Engine.Time % 1f > 0.5f ? "|" : "")}", Font.Small, pos);
			}
		}
	}
}