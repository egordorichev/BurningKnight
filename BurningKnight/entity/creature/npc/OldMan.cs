using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 18;

			AddComponent(new AnimationComponent("old_man"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));
			AddComponent(new CloseDialogComponent("old_man_0"));
			AddComponent(new InteractDialogComponent("old_man_1"));

			AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Area.Tags[Tags.BurningKnight].Count > 0) {
				Area.Tags[Tags.BurningKnight][0].Center = Center;
				Done = true;
			}
		}
	}
}