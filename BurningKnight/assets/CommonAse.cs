using BurningKnight.entity.item;
using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;

namespace BurningKnight.assets {
	public static class CommonAse {
		public static AnimationData Items;
		public static AnimationData Ui;
		public static AnimationData Projectiles;
		public static AnimationData Particles;
		public static AnimationData Props;
		
		public static TextureRegion Missing;
		
		public static void Load() {
			Items = Animations.Get("items");
			Ui = Animations.Get("ui");
			Projectiles = Animations.Get("projectiles");
			Particles = Animations.Get("particles");
			Props = Animations.Get("props");

			Missing = Items.GetSlice("missing");
			Textures.Missing = Missing;
			Item.UnknownRegion = Items.GetSlice("unknown");
		}
	}
}