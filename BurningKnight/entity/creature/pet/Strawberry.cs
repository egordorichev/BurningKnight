using BurningKnight.assets;
using BurningKnight.entity.component;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class Strawberry : Pet {
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice("bk:strawberry");

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new ZSliceComponent(CommonAse.Items, "bk:strawberry"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Float = true });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
		}
	}
}