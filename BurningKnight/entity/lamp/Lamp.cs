using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.lamp {
	public class Lamp : Entity {
		public Player Owner;
		public Item Item;

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new SliceComponent(CommonAse.Items, Item.Id));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new OrbitalComponent());
			
			Owner.GetComponent<OrbitGiverComponent>().AddOrbiter(this);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}