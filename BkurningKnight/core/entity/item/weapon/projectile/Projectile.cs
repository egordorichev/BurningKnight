using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.physics;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class Projectile : StatefulEntity {
		public Creature Owner;
		public int Damage = 1;
		public bool Bad;
		public bool Crit;
		public float Knockback = 2f;
		public bool Penetrates;
		protected bool Broke;
		public Body Body;
		protected bool DidDie;
		public static bool AllDie;
		public bool IgnoreBodyPos;
		protected bool IgnoreVel;
		public int I;
		protected bool IgnoreArmor = false;
		public bool NoPoof;

		public override Void Init() {
			base.Init();
			this.Bad = this.Owner is Mob;
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (AllDie && Bad) {
				Broke = true;
			} 

			if (Broke) {
				if (!DidDie) {
					DidDie = true;
					this.Death();
				} 

				return;
			} 

			if (this.Body != null && !IgnoreBodyPos) {
				this.X = this.Body.GetPosition().X;
				this.Y = this.Body.GetPosition().Y;
			} 

			this.Logic(Dt);

			if (this.Body != null && !this.IgnoreVel) {

			} 
		}

		public Void SetPos(float X, float Y) {
			this.X = X;
			this.Y = Y;

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(X, Y, this.Body.GetAngle());
			} 
		}

		public Void Brak() {
			this.Broke = true;
		}

		public override Void OnCollision(Entity Entity) {
			if (this.Broke) {
				return;
			} 

			if (this.BreaksFrom(Entity)) {
				this.Brak();

				return;
			} 

			if (this.Hit(Entity)) {
				this.Broke = !this.Penetrates;

				if (this.Owner != null && this.Owner.Penetrates) {
					this.Broke = false;
				} 
			} 
		}

		public Void Remove() {
			Broke = true;
		}

		protected bool BreaksFrom(Entity Entity) {
			return false;
		}

		protected bool Hit(Entity Entity) {
			return false;
		}

		protected Void DoHit(Entity Entity) {
			HpFx Fx = ((Creature) Entity).ModifyHp(-this.Damage, this, this.IgnoreArmor);
			((Creature) Entity).KnockBackFrom(this.Owner, this.Knockback);
			Graphics.Delay();

			if (Fx != null) {
				if (this.Crit) {
					Fx.Crit = true;
				} 
			} 

			this.OnHit(Entity);
		}

		protected Void Logic(float Dt) {

		}

		protected Void OnHit(Entity Entity) {

		}

		protected Void Death() {
			if (Done) {
				return;
			} 

			this.Done = true;
			this.OnDeath();

			if (!NoPoof) {
				for (int I = 0; I < 3; I++) {
					PoofFx Fx = new PoofFx();
					Fx.X = this.X;
					Fx.Y = this.Y;
					Fx.T += 0.5f;
					Dungeon.Area.Add(Fx);
				}
			} 
		}

		protected Void OnDeath() {

		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level || (Entity is Projectile && ((Projectile) Entity).Bad == this.Bad)) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}
