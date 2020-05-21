using BurningKnight.entity.component;

namespace BurningKnight.entity.item {
	public class RoundItem : Item {
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
	}
}