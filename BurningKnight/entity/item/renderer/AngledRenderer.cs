using System;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.entity.item.renderer {
	public class AngledRenderer : ItemRenderer {
		public double Angle;
		public float MaxAngle;
		public bool Stay;
		
		public AngledRenderer(float maxAngle, bool stay = false) {
			MaxAngle = maxAngle.ToRadians();
			Stay = stay;
		}
		
		public override void Render(bool atBack) {
			var region = Item.Region;
			var owner = Item.Owner;
			var flipped = owner.GraphicsComponent.Flipped;
			
			var angle = Angle + owner.AngleTo(Input.Mouse.GamePosition);

			if (flipped) {
				angle -= Math.PI; // - angle;
			}
			
			Graphics.Render(region, new Vector2(owner.CenterX, owner.Bottom - 4), (float) angle, new Vector2(region.Center.X, region.Source.Height),
				Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);	
		}

		public override void OnUse() {
			Tween.To(this, new { Angle = Angle > 1 ? 0 : MaxAngle }, 0.1f);
		}
	}
}