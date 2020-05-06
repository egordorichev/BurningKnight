using System;
using System.Collections.Generic;
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

		private static bool locked;
		private static List<Body> toRemove = new List<Body>();

		public static void Init() {
			World = new World(Vector2.Zero);
			Debug = new PhysicsDebugRenderer(World);

			World.ContactManager.PreSolve += PreSolve;

			World.ContactManager.BeginContact += BeginContact;
			World.ContactManager.EndContact += EndContact;
		}

		
		
		private static void RemoveBodies() {
			if (locked || World == null || toRemove.Count == 0) {
				return;
			}

			foreach (var b in toRemove) {
				try {
					World.RemoveBody(b);
				} catch (Exception e) {
					Log.Error(e);
				}
			}

			toRemove.Clear();
		}

		public static void RemoveBody(Body body) {
			if (!toRemove.Contains(body)) {
				toRemove.Add(body);
			}
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
			if (World == null) {
				return;
			}
			
			try {
				RemoveBodies();
				
				locked = true;
				World.Step(dt);
				locked = false;
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
			if (locked) {
				Log.Error("World was locked when destroying");
			}
			
			try {
				RemoveBodies();
				World?.Clear();
			} catch (Exception e) {
				Log.Error(e);
			}

			World = null;
		}
	}
}