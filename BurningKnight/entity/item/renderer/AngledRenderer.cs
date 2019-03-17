using System;
using BurningKnight.util;
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

		private double lastAngle;
		
		public AngledRenderer(float maxAngle, bool stay = false) {
			MaxAngle = maxAngle.ToRadians();
			Stay = stay;
		}
		
		public override void Render(bool atBack, bool paused, float dt) {
			var region = Item.Region;
			var owner = Item.Owner;
			var flipped = owner.GraphicsComponent.Flipped;

			if (!atBack && !paused) {
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(Input.Mouse.GamePosition), dt * 6f);
			}
			
			var angle = (flipped ? -Angle : Angle) + (atBack ? Math.PI / 4 : lastAngle);

			if (flipped) {
				angle -= Math.PI;
			}
			
			Graphics.Render(region, new Vector2(owner.CenterX + (flipped ? -3 : 3), owner.Bottom - 6), (float) angle, new Vector2(region.Center.X, region.Source.Height),
				new Vector2(flipped ? -1 : 1, 1));
		}

		public override void OnUse() {
			Tween.To(this, new { Angle = Angle > 1 ? 0 : MaxAngle }, 0.1f);
		}
	}
}