using Lens.entity;

namespace BurningKnight.level {
	public class Chasm : Entity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ChasmBodyComponent());
		}
	}
}