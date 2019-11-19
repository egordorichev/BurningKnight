using BurningKnight.entity.component;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.building {
	public class House : SolidProp {
		private TextureRegion shadow;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 141;
			Height = 164;
			Sprite = "house";

			shadow = Animations.Get("buildings").GetSlice("house_shadow");
			
			AddComponent(new ShadowComponent(RenderShadow));
		}

		protected override GraphicsComponent CreateGraphicsComponent() {
			return new SliceComponent("buildings", Sprite);
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(4, 84, 132, 75);
		}

		private void RenderShadow() {
			
			Width = 141;
			Height = 164;
			Graphics.Render(shadow, Position + new Vector2(0, 5 + Height), 0, Vector2.Zero, MathUtils.InvertY);
		}
	}
}