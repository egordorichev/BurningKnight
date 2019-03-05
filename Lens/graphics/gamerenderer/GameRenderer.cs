using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.gamerenderer {
	public class GameRenderer {
		public SpriteSortMode SpriteSortMode = SpriteSortMode.Deferred;
		public BlendState BlendState = BlendState.AlphaBlend;
		public SamplerState SamplerState = SamplerState.PointClamp;
		public DepthStencilState DepthStencilState = DepthStencilState.None;
		public RasterizerState DefaultRasterizerState = RasterizerState.CullNone;
		public RasterizerState ClipRasterizerState = new RasterizerState {
			ScissorTestEnable = true
		};
		
		public Effect Effect;
		public Color Bg = Color.Black;
		
		public virtual void Render() {
			
		}

		public virtual void Destroy() {
			
		}
	}
}