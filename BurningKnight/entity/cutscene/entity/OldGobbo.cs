using BurningKnight.entity.component;

namespace BurningKnight.entity.cutscene.entity {
	public class OldGobbo : CutsceneEntity {
		public bool RunAway;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 11;
			Height = 14;
			
			AddComponent(new AnimationComponent("old_gobbo"));
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (RunAway) {
				X += dt * 60;
			}
		}
	}
}