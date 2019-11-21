using Lens.graphics;
using Lens.util;

namespace BurningKnight.ui.str {
	public class IconRenderer : StrRenderer {
		public TextureRegion Region;
		
		public override int GetWidth(UiString str) {
			if (Region == null && str.Icons.Count > Id) {
				Region = str.Icons[Id];

				if (Region == null) {
					Log.Error($"Unknown icon {Id}");
				}
			}

			if (Region != null) {
				return (int) Region.Width;
			}
			
			return 0;
		}
	}
}