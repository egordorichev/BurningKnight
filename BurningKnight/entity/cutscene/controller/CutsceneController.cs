using System;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.input;

namespace BurningKnight.entity.cutscene.controller {
	public class CutsceneController : Entity {
		public DialogComponent Current;
		public CutsceneState State;

		public override void Init() {
			base.Init();
			AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Current == null) {
				return;
			}

			var dd = Current.Dialog;
			var controller = GamepadComponent.Current;
			
			if (dd.Saying && !dd.JustStarted) {
				if (Input.WasPressed(Controls.Interact, controller, true) || Input.WasPressed(Controls.UiSelect, controller, true)) {
					if (dd.DoneSaying) {
						dd.Finish();

						var c = Current;
						c.FinishCallback?.Invoke();
						c.FinishCallback = null;
					} else {
						dd.Str.FinishTyping();
					}
				}
			}
		}

		public void Start(DialogComponent component, string what, Action onEnd) {
			Current = component;
			component.Start(what, null, onEnd);
		}
	}
}