using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.entity;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature {
	public class Mine : SaveableEntity {
		public static Animation Animations = Animation.Make("actor-mine", "-normal");
		private AnimationData Idle;
		private Body Body;

		public override Void Init() {
			base.Init();
			Idle = Animations.Get("idle");
			H = 9;
			W = 14;
		}

		public override Void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
		}

		public override Void OnCollision(Entity E) {
			base.OnCollision(E);

			if (E is Player) {
				PlaySfx("explosion");
				Done = true;
				Explosion.Make(X + W / 2, Y + H / 2, true);

				for (int I = 0; I < Dungeon.Area.GetEntities().Size(); I++) {
					Entity Entity = Dungeon.Area.GetEntities().Get(I);

					if (Entity is Creature) {
						Creature Creature = (Creature) Entity;

						if (Creature.GetDistanceTo(this.X + W / 2, this.Y + H / 2) < 24f) {
							if (!Creature.ExplosionBlock) {
								if (Creature is Player) {
									Creature.ModifyHp(-1000, this, true);
								} else {
									Creature.ModifyHp(-Math.Round(Random.NewFloatDice(20 / 3 * 2, 20)), this, true);
								}

							} 

							float A = (float) Math.Atan2(Creature.Y + Creature.H / 2 - this.Y - 8, Creature.X + Creature.W / 2 - this.X - 8);
							float KnockbackMod = Creature.KnockbackMod;
							Creature.Knockback.X += Math.Cos(A) * 10f * KnockbackMod;
							Creature.Knockback.Y += Math.Sin(A) * 10f * KnockbackMod;
						} 
					} else if (Entity is Chest) {
						if (Entity.GetDistanceTo(this.X + W / 2, this.Y + H / 2) < 24f) {
							((Chest) Entity).Explode();
						} 
					} else if (Entity is BombEntity) {
						BombEntity B = (BombEntity) Entity;
						float A = (float) Math.Atan2(B.Y - this.Y, B.X - this.X) + Random.NewFloat(-0.5f, 0.5f);
						B.Vel.X += Math.Cos(A) * 200f;
						B.Vel.Y += Math.Sin(A) * 200f;
					} 
				}
			} 
		}

		public override Void Update(float Dt) {
			if (Body == null) {
				Body = World.CreateSimpleBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, true);
				Body.SetTransform(X, Y, 0);
			} 

			base.Update(Dt);
			Idle.Update(Dt);
		}

		public override Void Render() {
			base.Render();
			Idle.Render(this.X, this.Y, false);
		}
	}
}
