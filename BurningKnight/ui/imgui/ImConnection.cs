using System.Numerics;

namespace BurningKnight.ui.imgui {
	public class ImConnection {
		public Vector2 Offset;
		public ImConnection ConnectedTo;
		public ImNode Parent;
		public bool Input;
		public bool Active = false;
	}
}