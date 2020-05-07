using BurningKnight.entity.creature.bk;

namespace BurningKnight.entity.cutscene.entity {
	public class Heinur : CutsceneEntity {
		public override void AddComponents() {
			base.AddComponents();

			Width = 42;
			Height = 42;
			
			AddComponent(new BkGraphicsComponent("heinur"));
		}
	}
}