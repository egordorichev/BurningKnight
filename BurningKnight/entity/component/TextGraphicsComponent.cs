using BurningKnight.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class TextGraphicsComponent : GraphicsComponent {
		private string text;
		public Color Color = Color.White;

		public byte A {
			get => Color.A;
			set => Color.A = value;
		}
		
		public TextGraphicsComponent(string str) {
			text = str;
		}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Print(text, Font.Medium, Entity.Position, 0, Vector2.Zero, Vector2.One, Graphics.ParseEffect(Flipped, FlippedVerticaly));
				return;
			}
			
			Graphics.Color = Color;
			Graphics.Print(text, Font.Medium, Entity.Position);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}