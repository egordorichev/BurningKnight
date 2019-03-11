using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.graphics.animation {
	public class ColorSet {
		public readonly Color[] From;
		public readonly Color[] To;
		public readonly int Id;

		internal ColorSet(Color[] from, Color[] to, int id) {
			From = from;
			To = to;
			Id = id;

			if (from.Length != to.Length) {
				Log.Error("Invalid colorset");
			}
		}
	}
}