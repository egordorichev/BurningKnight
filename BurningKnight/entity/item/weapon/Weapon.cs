using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.trap;
using BurningKnight.physics;

namespace BurningKnight.entity.item.weapon {
	public class Weapon : WeaponBase {
		protected float Added;
		protected Body Body;
		private bool Played;
		private bool Used;

		public override void Use() {
			base.Use();

			if (Body != null) Body = World.RemoveBody(Body);

			CreateHitbox();
		}

		public override void SecondUse() {
			base.SecondUse();
		}

		protected void CreateHitbox() {
			BodyDef Def = new BodyDef();
			Def.Type = BodyDef.BodyType.DynamicBody;
			Body = World.World.CreateBody(Def);
			PolygonShape Poly = new PolygonShape();
			int W = Region.GetRegionWidth();
			int H = Region.GetRegionHeight();
			Poly.Set({
				new Vector2((float) Math.Floor((double) -W / 2), 0), new Vector2((float) Math.Ceil((double) W / 2), 0), new Vector2((float) Math.Floor((double) -W / 2), H), new Vector2((float) Math.Ceil((double) W / 2), H)
			});
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
			Body.SetTransform(Owner.X, Owner.Y, 0);
			Poly.Dispose();
			Played = false;
		}

		public override void EndUse() {
			Body = World.RemoveBody(Body);
			Used = false;
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public void SetAdded(float Added) {
			this.Added = Added;
		}

		public void OnHit(Creature Creature) {
		}

		protected float GetAngle(Entity Entity) {
			if (Entity == null) {
				var Aim = Owner.GetAim();

				return Owner.GetAngleTo(Aim.X, Aim.Y);
			}

			return Owner.GetAngleTo(Entity.X + Entity.W / 2, Entity.Y + Entity.H / 2);
		}

		protected void KnockFrom(Entity Entity) {
			Owner.KnockBackFrom(Entity, 30f);
		}

		public override void OnCollision(Entity Entity) {
			if (!Played) {
				if (Entity == null) {
					Owner.PlaySfx("clink");
					Played = true;
					KnockFrom(Entity);

					return;
				}

				if (Entity is RollingSpike || Entity is SolidProp || Entity is Entrance || Entity is Chest) {
					Owner.PlaySfx("clink");
					Played = true;
					KnockFrom(Entity);

					return;
				}
			}

			if (Entity is Creature && Entity != Owner) {
				if (Used && !Penetrates && !Owner.Penetrates) return;

				var Creature = (Creature) Entity;

				if (Creature.IsDead() || Creature.IsUnhittable()) return;

				Creature.KnockBackFrom(Owner, 2);

				if (Creature.IsDead() || Creature is Mob && Owner is Mob && !((Mob) Owner).Stupid) return;

				Used = true;
				OnHit(Creature);
				int Damage = -Math.Max(Creature.GetDefense() + 1, RollDamage());
				Creature.ModifyHp(Damage, Owner);
			}
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level || Entity == Owner || Entity is Door || Entity is ItemHolder) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}