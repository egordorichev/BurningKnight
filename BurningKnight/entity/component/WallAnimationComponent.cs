using System;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class WallAnimationComponent : MobAnimationComponent {
		public float WallAngle;
		
		public WallAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
		}

		public WallAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
		}

		protected override void CallRender(Vector2 pos, bool shadow) {
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(0, region.Height / 2);
			var w = region.Width / 2f;
			
			Graphics.Render(region, pos + origin + new Vector2((float) (WallAngle / Math.PI * w * 2) - (WallAngle > Math.PI * 1.2f ? w * 2 : 0),  (float) -Math.Sin(WallAngle) * w), WallAngle, origin, Scale);
		}
	}
}