using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;

namespace BurningKnight.level {
	public class Chasm : Entity, CollisionFilterEntity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ChasmBodyComponent());
		}

		public bool ShouldCollide(Entity entity) {
			return !entity.TryGetComponent<TileInteractionComponent>(out var t) || t.Supports.Count == 0;
		}
	}
}