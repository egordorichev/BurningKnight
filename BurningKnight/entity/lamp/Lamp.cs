using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.lamp {
	public class Lamp : Entity {
		public Player Owner;
		public Item Item;

		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice(Item.Id);

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			AddComponent(new SliceComponent(CommonAse.Items, Item.Id));
			AddComponent(new ShadowComponent(RenderShadow));
			
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}