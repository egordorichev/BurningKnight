using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.entity;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature {
	public class Mine : SaveableEntity {
		public static Animation Animations = Animation.Make("actor-mine", "-normal");
		private Body Body;
		private AnimationData Idle;

		public override void Init() {
			base.Init();
			Idle = Animations.Get("idle");
			H = 9;
			W = 14;
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
		}

		public override void OnCollision(Entity E) {
			base.OnCollision(E);

			if (E is Player) {
				PlaySfx("explosion");
				Done = true;
				Explosion.Make(X + W / 2, Y + H / 2, true);

				for (var I = 0; I < Dungeon.Area.GetEntities().Size(); I++) {
					Entity Entity = Dungeon.Area.GetEntities().Get(I);

					if (Entity is Creature) {
						var Creature = (Creature) Entity;

						if (Creature.GetDistanceTo(this.X + W / 2, this.Y + H / 2) < 24f) {
							if (!Creature.ExplosionBlock) {
								if (Creature is Player)
									Creature.ModifyHp(-1000, this, true);
								else
									Creature.ModifyHp(-Math.Round(Random.NewFloatDice(20 / 3 * 2, 20)), this, true);
							}

							var A = (float) Math.Atan2(Creature.Y + Creature.H / 2 - this.Y - 8, Creature.X + Creature.W / 2 - this.X - 8);
							var KnockbackMod = Creature.KnockbackMod;
							Creature.Knockback.X += Math.Cos(A) * 10f * KnockbackMod;
							Creature.Knockback.Y += Math.Sin(A) * 10f * KnockbackMod;
						}
					}
					else if (Entity is Chest) {
						if (Entity.GetDistanceTo(this.X + W / 2, this.Y + H / 2) < 24f) ((Chest) Entity).Explode();
					}
					else if (Entity is BombEntity) {
						var B = (BombEntity) Entity;
						var A = (float) Math.Atan2(B.Y - this.Y, B.X - this.X) + Random.NewFloat(-0.5f, 0.5f);
						B.Vel.X += Math.Cos(A) * 200f;
						B.Vel.Y += Math.Sin(A) * 200f;
					}
				}
			}
		}

		public override void Update(float Dt) {
			if (Body == null) {
				Body = World.CreateSimpleBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, true);
				Body.SetTransform(X, Y, 0);
			}

			base.Update(Dt);
			Idle.Update(Dt);
		}

		public override void Render() {
			base.Render();
			Idle.Render(this.X, this.Y, false);
		}
	}
}