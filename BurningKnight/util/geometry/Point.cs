namespace BurningKnight.util.geometry {
	public class Point {
		public Point() {
			
		}
		
		public Point(int x, int y) {
			X = x;
			Y = y;
		}

		public Point(int x) {
			X = x;
			Y = x;
		}
		
		public int X;
		public int Y;
	}
}