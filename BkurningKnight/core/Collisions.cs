using BurningKnight.core.entity;
using BurningKnight.core.entity.creature.mob.forest;
using BurningKnight.core.entity.creature.mob.hall;
using BurningKnight.core.entity.creature.mob.ice;
using BurningKnight.core.entity.creature.mob.tech;
using BurningKnight.core.entity.level;

namespace BurningKnight.core {
	public class Collisions : ContactListener, ContactFilter {
		public static Fixture Last;

		public override Void BeginContact(Contact Contact) {
			Entity A = (Entity) Contact.GetFixtureA().GetBody().GetUserData();
			Entity B = (Entity) Contact.GetFixtureB().GetBody().GetUserData();

			if (A is Level && !(B is Hedgehog || B is Thief || B is Roller || B is Vacuum || B is Tank)) {
				return;
			} else if (B is Level && !(A is Hedgehog || A is Thief || A is Roller || A is Vacuum || A is Tank)) {
				return;
			} 

			if (A != null) {
				Last = Contact.GetFixtureB();
				A.OnCollision(B);
			} 

			if (B != null) {
				Last = Contact.GetFixtureA();
				B.OnCollision(A);
			} 
		}

		public override Void EndContact(Contact Contact) {
			Entity A = (Entity) Contact.GetFixtureA().GetBody().GetUserData();
			Entity B = (Entity) Contact.GetFixtureB().GetBody().GetUserData();

			if (A != null) {
				A.OnCollisionEnd(B);
			} 

			if (B != null) {
				B.OnCollisionEnd(A);
			} 
		}

		public override Void PreSolve(Contact Contact, Manifold OldManifold) {
			Object A = Contact.GetFixtureA().GetBody().GetUserData();
			Object B = Contact.GetFixtureB().GetBody().GetUserData();
			bool Should = true;

			if (A is Entity && !((Entity) A).ShouldCollide(B, Contact, Contact.GetFixtureB())) {
				Should = false;
			} 

			if (B is Entity && !((Entity) B).ShouldCollide(A, Contact, Contact.GetFixtureA())) {
				Should = false;
			} 

			if (!Should) {
				Contact.SetEnabled(false);
			} 
		}

		public override Void PostSolve(Contact Contact, ContactImpulse Impulse) {

		}

		public override bool ShouldCollide(Fixture FixtureA, Fixture FixtureB) {
			return true;
		}
	}
}
