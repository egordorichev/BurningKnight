using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace BurningKnight {
	public class Physics {
		public static World World;

		public void Init() {
			World = new World(new AABB(), Vec2.Zero, true);
		}

		public void Destroy() {
			World.Dispose();
			World = null;
		}
	}
}