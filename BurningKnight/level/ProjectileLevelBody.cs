using Lens.entity;

namespace BurningKnight.level {
	public class ProjectileLevelBody : Entity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ProjectileBodyComponent {
				Level = Level
			});
		}
	}
}