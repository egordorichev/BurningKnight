using BurningKnight.assets;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class CustomDoor : LockableDoor {
		private TextureRegion bar;
		private TextureRegion pad;
		
		/*
		 * iron lock draws weirdly on doors after opening a save (wrong depth or not rendered?)
		 */

		public CustomDoor() {
			OpenByDefault = true;
		}

		public override void PostInit() {
			base.PostInit();
			var p = GetPad();

			if (p != null) {
				pad = CommonAse.Props.GetSlice(p);
				
				Area.Add(new RenderTrigger(this, () => {
					Graphics.Render(pad, Position);
				}, -1));
			}
			
			var b = GetBar();

			if (b == null) {
				return;
			}
			
			bar = CommonAse.Props.GetSlice(b);
			Area.Add(new RenderTrigger(this, () => RenderFrame(false), Layers.FlyingMob));
		}

		protected override float GetShadowOffset() {
			return 0;
		}

		protected virtual string GetBar() {
			return null;
		}

		protected virtual string GetPad() {
			return null;
		}

		protected override void RenderShadow() {
			base.RenderShadow();

			if (bar != null) {
				RenderFrame(true);
			}
		}

		private void RenderFrame(bool shadow) {
			Graphics.Render(bar, shadow ? new Vector2(X, Bottom + Height) : Position, 0, Vector2.Zero, shadow ? MathUtils.InvertY : MathUtils.Normal);
		}
	}
}