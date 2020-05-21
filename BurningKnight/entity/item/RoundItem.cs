using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item {
	public class RoundItem : Item {
		public bool Transparent;
		
		protected override BodyComponent GetBody() {
			return GetComponent<CircleBodyComponent>();
		}

		protected override bool HasBody() {
			return HasComponent<CircleBodyComponent>();
		}

		protected override void RemoveBody() {
			RemoveComponent<CircleBodyComponent>();
		}
		
		protected override BodyComponent CreateBody() {
			return new CircleBodyComponent(0, 0, 8);
		}

		public override bool ShouldCollide(Entity entity) {
			return !Transparent && base.ShouldCollide(entity);
		}

		protected override bool ShouldInteract(Entity entity) {
			return !Transparent && base.ShouldInteract(entity);
		}
	}
}