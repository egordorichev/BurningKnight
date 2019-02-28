using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.physics {
	public class Physics {
		public static World World;
		public static PhysicsDebugRenderer Debug;
		public static bool RenderDebug = true;
		
		public static void Init() {
			World = new World(Vector2.Zero);
			Debug = new PhysicsDebugRenderer(World);
			// World.SetContactFilter(new EntityContactFilter());
			//World.SetContactListener(new EntityContactListener());
			// World.SetDebugDraw(new PhysicsDebugRenderer());			
		}

		public static void Update(float dt) {
			World.Step(dt);
		}

		public static void Render() {
			Debug.DrawDebugData();
		}

		public static void Destroy() {
			World = null;
		}
	}
}