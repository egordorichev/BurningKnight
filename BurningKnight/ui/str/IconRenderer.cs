using Lens.graphics;

namespace BurningKnight.ui.str {
	public class IconRenderer : StrRenderer {
		public TextureRegion Region;
		
		public override int GetWidth(UiString str) {
			if (Region == null && str.Icons.Count > Id) {
				Region = str.Icons[Id];
			}

			if (Region != null) {
				return (int) Region.Width;
			}
			
			return 0;
		}
	}
}