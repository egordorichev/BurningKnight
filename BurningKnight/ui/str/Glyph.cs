using System.Collections.Generic;
using BurningKnight.ui.str.@event;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.ui.str {
	public class Glyph {
		public BitmapFontGlyph G;
		public Color Color;
		public Vector2 Offset;
		public Vector2 Origin;
		public float Angle;
		public Vector2 Scale = Vector2.One;
		public SpriteEffects Effects;
		public List<GlyphEvent> Events = new List<GlyphEvent>();
		public bool State;
		
		public void Reset() {
			Color = ColorUtils.WhiteColor;
			Offset.X = 0;
			Offset.Y = 0;

			if (G.FontRegion != null) {
				Origin.X = G.FontRegion.Width / 2f;
				Origin.Y = G.FontRegion.Height / 2f;
			}
			
			Angle = 0;
			Scale.X = 1;
			Scale.Y = 1;
			Effects = SpriteEffects.None;
		}
	}
}