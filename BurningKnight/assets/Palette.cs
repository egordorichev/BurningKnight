using System.Collections.Generic;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets {
	public static class Palette {
		public static Dictionary<string, Color> Colors = new Dictionary<string, Color>();
		
		public static Color[] Default = new[] {
			ColorUtils.FromHex("#ff0040"),
			ColorUtils.FromHex("#131313"),
			ColorUtils.FromHex("#1b1b1b"),
			ColorUtils.FromHex("#272727"),
			ColorUtils.FromHex("#3d3d3d"),
			ColorUtils.FromHex("#5d5d5d"),
			ColorUtils.FromHex("#858585"),
			ColorUtils.FromHex("#b4b4b4"),
			ColorUtils.FromHex("#ffffff"),
			ColorUtils.FromHex("#c7cfdd"),
			ColorUtils.FromHex("#92a1b9"),
			ColorUtils.FromHex("#657392"),
			ColorUtils.FromHex("#424c6e"),
			ColorUtils.FromHex("#2a2f4e"),
			ColorUtils.FromHex("#1a1932"),
			ColorUtils.FromHex("#0e071b"),
			ColorUtils.FromHex("#1c121c"),
			ColorUtils.FromHex("#391f21"),
			ColorUtils.FromHex("#5d2c28"),
			ColorUtils.FromHex("#8a4836"),
			ColorUtils.FromHex("#bf6f4a"),
			ColorUtils.FromHex("#e69c69"),
			ColorUtils.FromHex("#f6ca9f"),
			ColorUtils.FromHex("#f9e6cf"),
			ColorUtils.FromHex("#edab50"),
			ColorUtils.FromHex("#e07438"),
			ColorUtils.FromHex("#c64524"),
			ColorUtils.FromHex("#8e251d"),
			ColorUtils.FromHex("#ff5000"),
			ColorUtils.FromHex("#ed7614"),
			ColorUtils.FromHex("#ffa214"),
			ColorUtils.FromHex("#ffc825"),
			ColorUtils.FromHex("#ffeb57"),
			ColorUtils.FromHex("#d3fc7e"),
			ColorUtils.FromHex("#99e65f"),
			ColorUtils.FromHex("#5ac54f"),
			ColorUtils.FromHex("#33984b"),
			ColorUtils.FromHex("#1e6f50"),
			ColorUtils.FromHex("#134c4c"),
			ColorUtils.FromHex("#0c2e44"),
			ColorUtils.FromHex("#00396d"),
			ColorUtils.FromHex("#0069aa"),
			ColorUtils.FromHex("#0098dc"),
			ColorUtils.FromHex("#00cdf9"),
			ColorUtils.FromHex("#0cf1ff"),
			ColorUtils.FromHex("#94fdff"),
			ColorUtils.FromHex("#fdd2ed"),
			ColorUtils.FromHex("#f389f5"),
			ColorUtils.FromHex("#db3ffd"),
			ColorUtils.FromHex("#7a09fa"),
			ColorUtils.FromHex("#3003d9"),
			ColorUtils.FromHex("#0c0293"),
			ColorUtils.FromHex("#03193f"),
			ColorUtils.FromHex("#3b1443"),
			ColorUtils.FromHex("#622461"),
			ColorUtils.FromHex("#93388f"),
			ColorUtils.FromHex("#ca52c9"),
			ColorUtils.FromHex("#c85086"),
			ColorUtils.FromHex("#f68187"),
			ColorUtils.FromHex("#f5555d"),
			ColorUtils.FromHex("#ea323c"),
			ColorUtils.FromHex("#c42430"),
			ColorUtils.FromHex("#891e2b"),
			ColorUtils.FromHex("#571c27")
		};

		static Palette() {
			Define("red", Default[60]);
			Define("green", Default[36]);
			Define("blue", Default[41]);
			Define("yellow", Default[32]);
			Define("orange", Default[30]);
			Define("brown", Default[20]);
			Define("gray", Default[6]);
			Define("purple", Default[56]);
			Define("pink", Default[59]);
			Define("lime", Default[34]);
			Define("blue", Default[11]);
			Define("cyan", Default[43]);
		}

		public static void Define(string s, Color c) {
			Colors[s] = c;
		}
	}
}