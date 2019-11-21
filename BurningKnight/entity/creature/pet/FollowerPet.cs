using BurningKnight.assets;
using BurningKnight.entity.component;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class FollowerPet  : Pet {
		private string sprite;

		public FollowerPet(string spr) {
			sprite = spr;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice(sprite);

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new ZSliceComponent(CommonAse.Items, sprite));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Float = true });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
		}
	}
}