using Lens;
using Lens.assets;
using Lens.graphics.gamerenderer;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.assets {
	public class Shaders {
		public static Effect Ui;
		public static Effect Entity;
		public static Effect Terrain;
		public static Effect Screen;
		public static Effect Fog;
		public static Effect Chasm;
		public static Effect Item;
		public static Effect Bk;
		
		public static void Load() {
			Ui = Effects.Get("ui");
			Entity = Effects.Get("entity");
			Terrain = Effects.Get("terrain");
			Screen = Effects.Get("screen");
			Fog = Effects.Get("fog");
			Chasm = Effects.Get("chasm");
			Item = Effects.Get("item");
			Bk = Effects.Get("bk");

			Engine.Instance.StateRenderer.GameEffect = Screen;
			// Engine.Instance.StateRenderer.UiEffect = Ui;
		}

		public static void Begin(Effect effect) {
			var state = Engine.Instance.StateRenderer;
			
			state.End();
			state.SurfaceEffect = effect;
			state.SpriteSortMode = SpriteSortMode.Immediate;
			effect.CurrentTechnique.Passes[0].Apply();
			state.Begin();
		}

		public static void End() {
			var state = Engine.Instance.StateRenderer;
			
			state.End();
			state.SpriteSortMode = GameRenderer.DefaultSortMode;
			state.SurfaceEffect = null;
			state.Begin();
		}
	}
}