using BurningKnight.assets;
using BurningKnight.entity.component;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class TheKey : Pet {
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice("bk:the_key");

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new ZSliceComponent(CommonAse.Items, "bk:the_key"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Z = 2 });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}
	}
}