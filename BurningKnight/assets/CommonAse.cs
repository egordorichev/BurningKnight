using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;

namespace BurningKnight.assets {
	public static class CommonAse {
		public static AnimationData Items;
		public static AnimationData Fx;
		public static AnimationData Ui;
		public static AnimationData Projectiles;
		
		public static TextureRegion Missing;
		
		public static void Load() {
			Items = Animations.Get("items");
			Fx = Animations.Get("fx");
			Ui = Animations.Get("ui");
			Projectiles = Animations.Get("projectiles");

			Missing = Items.GetSlice("missing");
			Textures.Missing = Missing;
		}
	}
}