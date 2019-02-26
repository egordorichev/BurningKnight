using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.physics;

namespace BurningKnight.entity.item.weapon.projectile {
	public class Projectile : StatefulEntity {
		public static bool AllDie;
		public bool Bad;
		public Body Body;
		protected bool Broke;
		public bool Crit;
		public int Damage = 1;
		protected bool DidDie;
		public int I;
		protected bool IgnoreArmor = false;
		public bool IgnoreBodyPos;
		protected bool IgnoreVel;
		public float Knockback = 2f;
		public bool NoPoof;
		public Creature Owner;
		public bool Penetrates;

		public override void Init() {
			base.Init();
			Bad = Owner is Mob;
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (AllDie && Bad) Broke = true;

			if (Broke) {
				if (!DidDie) {
					DidDie = true;
					Death();
				}

				return;
			}

			if (Body != null && !IgnoreBodyPos) {
				this.X = Body.GetPosition().X;
				this.Y = Body.GetPosition().Y;
			}

			Logic(Dt);

			if (Body != null && !IgnoreVel) {
			}
		}

		public void SetPos(float X, float Y) {
			this.X = X;
			this.Y = Y;

			if (Body != null) World.CheckLocked(Body).SetTransform(X, Y, Body.GetAngle());
		}

		public void Brak() {
			Broke = true;
		}

		public override void OnCollision(Entity Entity) {
			if (Broke) return;

			if (BreaksFrom(Entity)) {
				Brak();

				return;
			}

			if (Hit(Entity)) {
				Broke = !Penetrates;

				if (Owner != null && Owner.Penetrates) Broke = false;
			}
		}

		public void Remove() {
			Broke = true;
		}

		protected bool BreaksFrom(Entity Entity) {
			return false;
		}

		protected bool Hit(Entity Entity) {
			return false;
		}

		protected void DoHit(Entity Entity) {
			var Fx = ((Creature) Entity).ModifyHp(-Damage, this, IgnoreArmor);
			((Creature) Entity).KnockBackFrom(Owner, Knockback);
			Graphics.Delay();

			if (Fx != null)
				if (Crit)
					Fx.Crit = true;

			OnHit(Entity);
		}

		protected void Logic(float Dt) {
		}

		protected void OnHit(Entity Entity) {
		}

		protected void Death() {
			if (Done) return;

			Done = true;
			OnDeath();

			if (!NoPoof)
				for (var I = 0; I < 3; I++) {
					var Fx = new PoofFx();
					Fx.X = this.X;
					Fx.Y = this.Y;
					Fx.T += 0.5f;
					Dungeon.Area.Add(Fx);
				}
		}

		protected void OnDeath() {
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level || Entity is Projectile && ((Projectile) Entity).Bad == Bad) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}