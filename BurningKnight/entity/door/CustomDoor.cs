using BurningKnight.assets;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.door {
	public class CustomDoor : LockableDoor {
		private TextureRegion bar;

		public override void PostInit() {
			base.PostInit();
			
			bar = CommonAse.Props.GetSlice(GetBar());
			Area.Add(new RenderTrigger(this, () => RenderFrame(false), Layers.FlyingMob));
		}

		protected virtual string GetBar() {
			return "";
		}

		protected override void RenderShadow() {
			base.RenderShadow();
			RenderFrame(true);
		}

		private void RenderFrame(bool shadow) {
			Graphics.Render(bar, shadow ? new Vector2(X, Bottom + Height) : Position, 0, Vector2.Zero, shadow ? MathUtils.InvertY : MathUtils.Normal);
		}
	}
}