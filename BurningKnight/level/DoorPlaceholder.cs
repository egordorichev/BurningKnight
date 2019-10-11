using BurningKnight.util.geometry;

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
		
		public DoorPlaceholder(Dot P) {
			X = P.X;
			Y = P.Y;
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