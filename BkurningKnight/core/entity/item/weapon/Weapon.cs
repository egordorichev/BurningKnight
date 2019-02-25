using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon {
	public class Weapon : WeaponBase {
		protected Body Body;
		private bool Used = false;
		protected float Added;
		private bool Played;

		public override Void Use() {
			base.Use();

			if (this.Body != null) {
				this.Body = World.RemoveBody(this.Body);
			} 

			this.CreateHitbox();
		}

		public override Void SecondUse() {
			base.SecondUse();
		}

		protected Void CreateHitbox() {
			BodyDef Def = new BodyDef();
			Def.Type = BodyDef.BodyType.DynamicBody;
			Body = World.World.CreateBody(Def);
			PolygonShape Poly = new PolygonShape();
			int W = this.Region.GetRegionWidth();
			int H = this.Region.GetRegionHeight();
			Poly.Set({ new Vector2((float) Math.Floor((double) -W / 2), 0), new Vector2((float) Math.Ceil((double) W / 2), 0), new Vector2((float) Math.Floor((double) -W / 2), H), new Vector2((float) Math.Ceil((double) W / 2), H) });
			FixtureDef Fixture = new FixtureDef();
			Fixture.Shape = Poly;
			Fixture.Friction = 0;
			Fixture.IsSensor = false;
			Fixture.Filter.CategoryBits = 0x0002;
			Fixture.Filter.GroupIndex = -1;
			Fixture.Filter.MaskBits = -1;
			Body.CreateFixture(Fixture);
			Body.SetUserData(this);
			Body.SetBullet(true);
			Body.SetTransform(this.Owner.X, this.Owner.Y, 0);
			Poly.Dispose();
			Played = false;
		}

		public override Void EndUse() {
			this.Body = World.RemoveBody(this.Body);
			this.Used = false;
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public Void SetAdded(float Added) {
			this.Added = Added;
		}

		public Void OnHit(Creature Creature) {

		}

		protected float GetAngle(Entity Entity) {
			if (Entity == null) {
				Point Aim = this.Owner.GetAim();

				return this.Owner.GetAngleTo(Aim.X, Aim.Y);
			} 

			return this.Owner.GetAngleTo(Entity.X + Entity.W / 2, Entity.Y + Entity.H / 2);
		}

		protected Void KnockFrom(Entity Entity) {
			this.Owner.KnockBackFrom(Entity, 30f);
		}

		public override Void OnCollision(Entity Entity) {
			if (!Played) {
				if (Entity == null) {
					this.Owner.PlaySfx("clink");
					Played = true;
					this.KnockFrom(Entity);

					return;
				} else if (Entity is RollingSpike || Entity is SolidProp || Entity is Entrance || Entity is Chest) {
					this.Owner.PlaySfx("clink");
					Played = true;
					this.KnockFrom(Entity);

					return;
				} 
			} 

			if (Entity is Creature && Entity != this.Owner) {
				if (this.Used && (!this.Penetrates && !this.Owner.Penetrates)) {
					return;
				} 

				Creature Creature = (Creature) Entity;

				if (Creature.IsDead() || Creature.IsUnhittable()) {
					return;
				} 

				Creature.KnockBackFrom(this.Owner, 2);

				if (Creature.IsDead() || ((Creature is Mob && this.Owner is Mob && !((Mob) this.Owner).Stupid))) {
					return;
				} 

				this.Used = true;
				this.OnHit(Creature);
				int Damage = -Math.Max(Creature.GetDefense() + 1, this.RollDamage());
				Creature.ModifyHp(Damage, this.Owner);
			} 
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if ((Entity is Level) || Entity == Owner || Entity is Door || Entity is ItemHolder) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}
