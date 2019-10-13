using Lens.assets;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.assets {
	public class Font {
		public static BitmapFont Small;
		public static BitmapFont Medium;
		
		public static void Load() {
			Small = LoadFont("Fonts/small_font");
			Medium = LoadFont("Fonts/large_font");
		}

		public static BitmapFont LoadFont(string name) {
			return Assets.Content.Load<BitmapFont>(name);
		}
	}
}