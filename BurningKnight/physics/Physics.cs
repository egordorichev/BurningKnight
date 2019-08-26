using System;
using BurningKnight.entity.component;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Collision.Narrowphase;
using VelcroPhysics.Dynamics;

namespace BurningKnight.physics {
	public class Physics {
		public static World World;
		public static PhysicsDebugRenderer Debug;
		public static bool RenderDebug = false;

		public static Fixture Fixture;

		public static void Init() {
			World = new World(Vector2.Zero);
			Debug = new PhysicsDebugRenderer(World);
			
			World.ContactManager.PreSolve += PreSolve;
			
			World.ContactManager.BeginContact += BeginContact;
			World.ContactManager.EndContact += EndContact;
		}
		
		public static void PreSolve(Contact contact, ref Manifold oldManifold) {
			var a = contact.FixtureA.Body.UserData;
			var b = contact.FixtureB.Body.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				if (ac.Entity.TryGetComponent<CollisionFilterComponent>(out var af)) {
					var v = af.Invoke(bc.Entity);

					if (v == CollisionResult.Disable) {
						contact.Enabled = false;
					} else if (v == CollisionResult.Enable) {
						return;
					}
				}
				
				if (bc.Entity.TryGetComponent<CollisionFilterComponent>(out var bf)) {
					var v = bf.Invoke(ac.Entity);

					if (v == CollisionResult.Disable) {
						contact.Enabled = false;
					} else if (v == CollisionResult.Enable) {
						return;
					}
				}
				
				if (!ac.ShouldCollide(bc.Entity) || !bc.ShouldCollide(ac.Entity)) {
					contact.Enabled = false;
				}
			}
		}
		
		public static bool BeginContact(Contact contact) {
			var a = contact.FixtureA.Body.UserData;
			var b = contact.FixtureB.Body.UserData;
			
			if (a is BodyComponent ac && b is BodyComponent bc) {
				Fixture = contact.FixtureB;
				ac.OnCollision(bc.Entity);
			
				Fixture = contact.FixtureA;
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
			try {
				World?.Step(dt);
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public static void Render() {
			if (RenderDebug) {
				Debug.DrawDebugData();
			}
		}

		public static void Destroy() {
			World?.Clear();
			World = null;
		}
	}
}