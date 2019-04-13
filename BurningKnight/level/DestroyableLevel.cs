using Lens.entity;

namespace BurningKnight.level {
	public class DestroyableLevel : Entity {
		public Level Level;
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new DestroyableBodyComponent());
		}
	}
}