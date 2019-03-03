using Lens.entity.component.graphics;

namespace BurningKnight.entity.creature.player {
	public class PlayerHatComponent : AnimationComponent {
		public PlayerHatComponent() : base("gobbo", "gobbo") {
			// todo: implement hat selection
		}

		public override void Update(float dt) {
			base.Update(dt);

			string current = Entity.GetComponent<AnimationComponent>().Animation.Tag;

			if (Animation.Tag != current) {
				Animation.Tag = current;
			}
		}
	}
}