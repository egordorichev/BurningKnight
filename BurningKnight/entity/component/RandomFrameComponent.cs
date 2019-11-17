using System.Linq;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class RandomFrameComponent : GraphicsComponent {
		public TextureRegion Sprite;
		public Color Tint = ColorUtils.WhiteColor;
		
		public RandomFrameComponent(string anim) {
			var list = Animations.Get(anim).Layers.First().Value;
			Sprite = list[Rnd.Int(list.Count)].Texture;
		}
		
		public override void Render(bool shadow) {
			Graphics.Color = Tint;

			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height), 0, Vector2.Zero, Vector2.One, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
			} else {
				var pos = Entity.Position;

				if (Entity.TryGetComponent<ZComponent>(out var c)) {
					pos.Y -= c.Z;
				}
			
				Graphics.Render(Sprite, pos);
			}
			
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}