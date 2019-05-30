using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.entity;
using Microsoft.Xna.Framework;
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
			
			AddComponent(new ZSliceComponent(CommonAse.Items, Item.Id));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Z = 2 });
			AddComponent(new LightComponent(this, 32f, new Color(1f, 0.3f, 0.3f, 0.2f)));
			
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}