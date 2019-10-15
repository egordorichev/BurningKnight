using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class MaPuzzle : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 8;
			
			AddComponent(new AnimationComponent("mapuzzle"));
			AddComponent(new CloseDialogComponent("mapuzzle_0"));
		}
	}
}