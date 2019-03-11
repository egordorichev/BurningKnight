using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Lens.graphics.animation {
	public static class ColorSets {
		public static List<ColorSet> Colors = new List<ColorSet>();

		public static ColorSet New(Color[] from, Color[] to) {
			var set = new ColorSet(from, to, Colors.Count);
			Colors.Add(set);

			return set;
		}
	}
}