using Microsoft.Xna.Framework.Graphics;

namespace Lens.Graphics.GameRenderer {
	public class GameRenderer {
		public SpriteSortMode SpriteSortMode = SpriteSortMode.Deferred;
		public BlendState BlendState = BlendState.AlphaBlend;
		public SamplerState SamplerState = SamplerState.PointClamp;
		public DepthStencilState DepthStencilState = DepthStencilState.None;
		public RasterizerState RasterizerState = RasterizerState.CullNone;
		public Effect Effect;
		
		public virtual void Render() {
			
		}

		public virtual void Destroy() {
			
		}
	}
}