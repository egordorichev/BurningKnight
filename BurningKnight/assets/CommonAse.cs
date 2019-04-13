using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;

namespace BurningKnight.assets {
	public static class CommonAse {
		public static AnimationData Items;
		public static AnimationData Fx;
		public static AnimationData Ui;
		public static TextureRegion Missing;
		
		public static void Load() {
			Items = Animations.Get("items");
			Fx = Animations.Get("fx");
			Ui = Animations.Get("ui");

			Missing = Items.GetSlice("missing");
			Textures.Missing = Missing;
		}
	}
}