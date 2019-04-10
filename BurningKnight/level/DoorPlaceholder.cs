using Microsoft.Xna.Framework;

namespace BurningKnight.level {
	public class DoorPlaceholder {
		private Variant type = Variant.Empty;
		
		public Variant Type {
			get => type;

			set {
				if (value.CompareTo(type) > 0) {
					type = value;
				}
			}
		}
		
		public int X;
		public int Y;
		
		public DoorPlaceholder(Vector2 P) {
			X = (int) P.X;
			Y = (int) P.Y;
		}

		public enum Variant {
			Empty,
			Tunnel,
			Regular,
			Maze,
			Enemy,
			Locked,
			Boss,
			Secret
		}
	}
}