using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.gamerenderer {
	public class GameRenderer {
		public SpriteSortMode SpriteSortMode = SpriteSortMode.Immediate;
		public BlendState BlendState = BlendState.NonPremultiplied;
		public SamplerState SamplerState = SamplerState.PointClamp;
		public DepthStencilState DepthStencilState = DepthStencilState.None;
		public RasterizerState DefaultRasterizerState = RasterizerState.CullNone;
		public RasterizerState ClipRasterizerState = new RasterizerState {
			ScissorTestEnable = true
		};
		
		public Effect GameEffect;
		public Effect UiEffect;
		public Effect SurfaceEffect;
		public Color Bg = Color.Black;

		public virtual void Begin() {
			
		}

		public virtual void End() {
			
		}
		
		public virtual void Render() {
			
		}

		public virtual void Destroy() {
			
		}

		public virtual void Resize(int width, int height) {
			
		}
	}
}