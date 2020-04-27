using BurningKnight.assets;
using BurningKnight.entity.component;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class FollowerPet  : Pet {
		protected string Sprite;

		public FollowerPet(string spr) {
			Sprite = spr;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice(Sprite);

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new ZSliceComponent(CommonAse.Items, Sprite));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Float = true });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			GetComponent<ZSliceComponent>().Animate();
		}
	}
}