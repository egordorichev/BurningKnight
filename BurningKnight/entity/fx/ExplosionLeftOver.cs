using BurningKnight.entity.component;
using Lens.entity;
using Lens.entity.component.graphics;

namespace BurningKnight.entity.fx {
	public class ExplosionLeftOver : Entity {
		public ExplosionLeftOver() {
			Width = 38;
			Height = 38;
			Depth = Layers.FloorParticles;
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new SliceComponent("props", "explosion_leftover"));
		}
	}
}