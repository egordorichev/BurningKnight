using BurningKnight.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiLabel : UiEntity {
		protected Vector2 origin;
		protected string label;
		
		public string Label {
			get => label;

			set {
				if (label != value) {
					label = value;
					
					var size = Font.Medium.MeasureString(label);

					Width = size.Width;
					Height = size.Height;
					
					origin = new Vector2(Width / 2, Height / 2);
				}
			}
		}

		public float Angle;
		public Vector2 Scale = Vector2.One;

		public override void Render(Vector2 position) {
			Graphics.Print(label, Font.Small, position, Angle, origin, Scale);
		}
	}
}