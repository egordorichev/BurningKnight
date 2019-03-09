using Lens;
using Lens.assets;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.assets {
	public class Shaders {
		public static Effect Creature;
		
		public static void Load() {
			Creature = Effects.Get("creature");
		}

		public static void Begin(Effect effect) {
			var state = Engine.Instance.StateRenderer;
			
			state.End();
			state.Effect = effect;
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