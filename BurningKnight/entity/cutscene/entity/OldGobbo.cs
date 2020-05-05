using BurningKnight.entity.component;

namespace BurningKnight.entity.cutscene.entity {
	public class OldGobbo : CutsceneEntity {
		public override void AddComponents() {
			base.AddComponents();

			Width = 11;
			Height = 14;
			
			AddComponent(new AnimationComponent("old_gobbo"));
		}
	}
}