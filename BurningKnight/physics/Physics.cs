using BurningKnight.entity.component;
using Microsoft.Xna.Framework;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Dynamics;

namespace BurningKnight.physics {
	public class Physics {
		public static World World;
		public static PhysicsDebugRenderer Debug;
		public static bool RenderDebug = false;

		public static void Init() {
			World = new World(Vector2.Zero);
			Debug = new PhysicsDebugRenderer(World);
			
			World.ContactManager.ContactFilter += ShouldCollide;
			
			World.ContactManager.BeginContact += BeginContact;
			World.ContactManager.EndContact += EndContact;
		}
		
		public static bool ShouldCollide(Fixture fixtureA, Fixture fixtureB) {
			var a = fixtureA.Body.UserData;
			var b = fixtureB.Body.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				if (!ac.ShouldCollide(bc.Entity)) {
					return false;
				}
				
				if (!bc.ShouldCollide(ac.Entity)) {
					return false;
				}
			}

			return true;
		}
		
		public static bool BeginContact(Contact contact) {
			var a = contact.FixtureA.Body.UserData;
			var b = contact.FixtureB.Body.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				ac.OnCollision(bc.Entity);
				bc.OnCollision(ac.Entity);
			}

			return true;
		}

		public static void EndContact(Contact contact) {
			var a = contact.FixtureA.Body.UserData;
			var b = contact.FixtureB.Body.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				ac.OnCollisionEnd(bc.Entity);
				bc.OnCollisionEnd(ac.Entity);
			}
		}

		public static void Update(float dt) {
			World.Step(dt);
		}

		public static void Render() {
			if (RenderDebug) {
				Debug.DrawDebugData();
			}
		}

		public static void Destroy() {
			World = null;
		}
	}
}