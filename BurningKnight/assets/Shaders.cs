using Lens;
using Lens.assets;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.assets {
	public class Shaders {
		public static Effect Entity;
		
		public static void Load() {
			Entity = Effects.Get("entity");
		}

		public static void Begin(Effect effect) {
			var state = Engine.Instance.StateRenderer;
			
			state.End();
			state.Effect = effect;
			effect.CurrentTechnique.Passes[0].Apply();
			state.Begin();
		}

		public static void End() {
			var state = Engine.Instance.StateRenderer;
			
			state.End();
			state.Effect = null;
			state.Begin();
		}
	}
}