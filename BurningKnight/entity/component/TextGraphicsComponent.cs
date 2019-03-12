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

		public override void Render() {
			Graphics.Color = Color;
			Graphics.Print(text, Font.Medium, Entity.Position);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}