using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.input;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 18;

			AddComponent(new AnimationComponent("old_man"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));

			if (Run.Depth == 0) {
				AddComponent(new CloseDialogComponent("old_man_0"));
				AddComponent(new InteractDialogComponent("old_man_1"));	
			} else if (GlobalSave.IsFalse("control_roll")) {
				set = false;
				AddComponent(new CloseDialogComponent("control_1"));
			}

			AlwaysActive = true;
		}

		private bool set = true;

		public override void Update(float dt) {
			base.Update(dt);

			if (!set) {
				set = true;
				GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Roll, GamepadComponent.Current != null));
			}
		}
	}
}