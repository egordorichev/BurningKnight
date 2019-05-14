using System.Collections.Generic;
using System.Numerics;

namespace BurningKnight.ui.imgui {
	public class ImConnection {
		public Vector2 Offset;
		public List<ImConnection> ConnectedTo = new List<ImConnection>();
		public ImNode Parent;
		public bool Input;
		public bool Active = false;
	}
}