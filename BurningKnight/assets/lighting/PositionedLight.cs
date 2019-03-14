using Microsoft.Xna.Framework;

namespace BurningKnight.assets.lighting {
	public class PositionedLight : Light {
		public Vector2 Position;

		public PositionedLight(byte id) : base(id) {}

		public override Vector2 GetPosition() {
			return Position;
		}
	}
}