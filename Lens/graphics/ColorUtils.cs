using Microsoft.Xna.Framework;

namespace Lens.graphics {
	public static class ColorUtils {
		public static Color FromHex(string hex) {
			Color color = new Color();

			color.R = (byte) int.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
			color.G = (byte) int.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
			color.B = (byte) int.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
			
			return color;
		}
	}
}