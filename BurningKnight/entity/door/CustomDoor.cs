using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

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

		private void RenderFrame(bool shadow) {
			Graphics.Render(bar, Position, 0, Vector2.Zero, shadow ? MathUtils.InvertY : MathUtils.Normal);
		}
	}
}