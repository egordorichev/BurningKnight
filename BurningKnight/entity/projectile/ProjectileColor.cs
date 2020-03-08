using System.Collections.Generic;
using BurningKnight.assets;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public static class ProjectileColor {
		public static Color Yellow = Palette.Default[30];
		public static Color Red = Palette.Default[0];
		public static Color Green = Palette.Default[35];
		public static Color Blue = Palette.Default[40];
		public static Color Cyan = Palette.Default[42];
		public static Color Purple = Palette.Default[54];
		public static Color Orange = Palette.Default[28];
		public static Color Pink = Palette.Default[59];
		public static Color Brown = Palette.Default[19];
		public static Color Black = Color.Black;
		public static Color White = Color.White;

		public static Color[] Rainbow = {
			Red, Orange, Yellow, Green, Cyan, Blue, Purple
		};

		public static Dictionary<string, Color> Colors = new Dictionary<string, Color> {
			{ "yellow", Yellow },
			{ "red", Red },
			{ "green", Green },
			{ "blue", Blue },
			{ "cyan", Cyan },
			{ "purple", Purple },
			{ "orange", Orange },
			{ "pink", Pink },
			{ "brown", Brown },
			{ "black", Black },
			{ "white", White },
		};
	}
}