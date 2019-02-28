using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace BurningKnight.physics {
	public class Physics {
		public static World World;
		public static bool RenderDebug;

		private const int VelocityIterations = 4;
		private const int PositionIterations = 4;
		
		public static void Init() {
			World = new World(new AABB(), Vec2.Zero, true);
			World.SetContactFilter(new EntityContactFilter());
			World.SetContactListener(new EntityContactListener());
		}

		public static void Update(float dt) {
			World.Step(dt, VelocityIterations, PositionIterations);
		}

		public static void Destroy() {
			World.Dispose();
			World = null;
		}
	}
}