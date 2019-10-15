using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class NullPtr : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 10;
			Height = 8;

			AddComponent(new AnimationComponent("nullptr"));
			AddComponent(new CloseDialogComponent("nullptr_0"));
		}
	}
}