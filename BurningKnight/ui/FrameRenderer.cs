using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class FrameRenderer : Entity {
		protected TextureRegion Top;
		protected TextureRegion TopLeft;
		protected TextureRegion TopRight;
		
		protected TextureRegion RCenter;
		protected TextureRegion CenterLeft;
		protected TextureRegion CenterRight;
		
		protected TextureRegion RBottom;
		protected TextureRegion BottomLeft;
		protected TextureRegion BottomRight;

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public void Setup(string ase, string prefix) {
			var anim = Animations.Get(ase);

			Top = anim.GetSlice($"{prefix}top");
			TopLeft = anim.GetSlice($"{prefix}top_left");
			TopRight = anim.GetSlice($"{prefix}top_right");
			
			RCenter = anim.GetSlice($"{prefix}center");
			CenterLeft = anim.GetSlice($"{prefix}left");
			CenterRight = anim.GetSlice($"{prefix}right");
			
			RBottom = anim.GetSlice($"{prefix}bottom");
			BottomLeft = anim.GetSlice($"{prefix}bottom_left");
			BottomRight = anim.GetSlice($"{prefix}bottom_right");
		}

		public override void Render() {
			base.Render();
			RenderFrame();
		}

		public virtual void RenderFrame() {
			Graphics.Render(TopLeft, Position);
			Graphics.Render(Top, new Vector2(X + TopLeft.Width, Y), 0, Vector2.Zero, 
				new Vector2((Width - TopLeft.Width - TopRight.Width) / (Top.Width), 1));
			Graphics.Render(TopRight, new Vector2(Right - TopRight.Width, Y));
			
			Graphics.Render(CenterLeft, new Vector2(X, Y + TopLeft.Height), 0, Vector2.Zero, 
				new Vector2(1, (Height - TopLeft.Height - BottomLeft.Height) / (CenterLeft.Height)));
			Graphics.Render(RCenter, new Vector2(X + CenterLeft.Width, Y + Top.Height), 0, Vector2.Zero, 
				new Vector2(
					(Width - CenterLeft.Width - CenterRight.Width) / (RCenter.Width), 
					(Height - Top.Height - RBottom.Height) / (RCenter.Height)
					));
			Graphics.Render(CenterRight, new Vector2(Right - CenterRight.Width, Y + TopRight.Height), 0, Vector2.Zero,
				new Vector2(1, (Height - TopRight.Height - BottomRight.Height) / (CenterRight.Height)));
			
			Graphics.Render(BottomLeft, new Vector2(X, Bottom - BottomLeft.Height));
			Graphics.Render(RBottom, new Vector2(X + BottomLeft.Width, Bottom - RBottom.Height), 0, Vector2.Zero, 
				new Vector2((Width - BottomLeft.Width - BottomRight.Width) / (RBottom.Width), 1));
			Graphics.Render(BottomRight, new Vector2(Right - BottomRight.Width, Bottom - BottomRight.Height));
		}
	}
}