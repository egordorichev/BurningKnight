using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.buff.fx;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.npc;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.entity {
	public class BombEntity : Entity {
		protected void _Init() {
			{
				Depth = -1;
				AlwaysActive = true;
			}
		}

		public static Animation Normal = Animation.Make("actor-bomb", "-normal");
		public static Animation Iced = Animation.Make("actor-bomb", "-ice_bomb");
		public static Animation Poisoned = Animation.Make("actor-bomb", "-poison");
		public static Animation Tiny = Animation.Make("actor-bomb", "-small");
		private AnimationData Animation;
		private Body Body;
		public Point Vel = new Point();
		public Creature Owner;
		public bool Bullets;
		public List<Buff> ToApply = new List<>();
		private PointLight Light;
		private bool Burning;
		private bool Poison;
		private bool Ice;
		public bool Small;
		private bool Fliped;
		private float Mod;
		private float T;
		public bool LeaveSmall;
		private float LastFlame;

		public BombEntity(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.Fliped = Random.Chance(50);
			Light = World.NewLight(32, new Color(1, 0, 0, 1), 64, 0, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		public override Void Init() {
			base.Init();
			Mod = Random.NewFloat(0.95f, 1.05f);

			foreach (Buff Buff in this.ToApply) {
				if (Buff is BurningBuff) {
					this.Burning = true;
				} else if (Buff is PoisonedBuff) {
					this.Poison = true;
				} else if (Buff is FrozenBuff) {
					this.Ice = true;
				} 
			}

			if (this.Small) {
				W = 8;
				H = 8;
			} else {
				W = 10;
				H = 14;
			}


			if (this.Ice) {
				this.Animation = Iced.Get("idle");
			} else if (this.Poison) {
				this.Animation = Poisoned.Get("idle");
			} else if (this.Small) {
				this.Animation = Tiny.Get("idle");
			} else {
				this.Animation = Normal.Get("idle");
			}


			this.Body = World.CreateSimpleBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, false);
			MassData Data = new MassData();
			Data.Mass = 0.1f;
			this.Body.SetMassData(Data);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.PlaySfx("bomb_placed");
			Room Room = Dungeon.Level.FindRoomFor(this.X, this.Y);

			if (Shopkeeper.Instance != null && Shopkeeper.Instance.Room == Room) {
				Shopkeeper.Instance.Enrage();
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(this.Light);
			this.Body = World.RemoveBody(this.Body);
		}

		public BombEntity ToMouseVel() {
			return this.VelTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y, 60f);
		}

		public BombEntity VelTo(float X, float Y, float F) {
			float A = (float) Math.Atan2(Y - this.Y - 8, X - this.X - 8);
			this.Vel = new Point((float) Math.Cos(A) * F, (float) Math.Sin(A) * F);
			this.X += Math.Cos(A) * 5f;
			this.Y += Math.Sin(A) * 5f;

			return this;
		}

		public override Void Update(float Dt) {
			this.T += Dt;

			if (this.Burning) {
				this.LastFlame += Dt;

				if (this.LastFlame >= 0.1f) {
					this.LastFlame = 0;
					Dungeon.Area.Add(new FlameFx(this));
				} 
			} 

			this.X = this.Body.GetPosition().X;
			this.Y = this.Body.GetPosition().Y;
			this.Vel.Mul(0.95f);
			this.Body.SetLinearVelocity(this.Vel);

			if (this.Animation.Update(Dt * this.Mod)) {
				if (this.Bullets) {
					for (int I = 0; I < 8; I++) {
						BulletProjectile Bullet = new NanoBullet();
						float F = 60;
						float A = (float) (I * (Math.PI / 4));
						Bullet.Damage = 10;
						Bullet.Bad = this.Owner is Mob;
						Bullet.X = (float) (this.X + Math.Cos(A) * 8);
						Bullet.Y = (float) (this.Y + Math.Sin(A) * 8);
						Bullet.Velocity.X = (float) (Math.Cos(A) * F);
						Bullet.Velocity.Y = (float) (Math.Sin(A) * F);
						Dungeon.Area.Add(Bullet);
					}
				} 

				if (this.LeaveSmall && !this.Small) {
					for (int I = 0; I < 4; I++) {
						BombEntity E = new BombEntity(this.X + this.W - Random.NewFloat(this.W), this.Y + this.H - Random.NewFloat(this.H));
						E.Small = true;
						float A = (float) (I * Math.PI / 2);
						float F = 200f;
						E.Vel = new Point((float) Math.Cos(A) * F, (float) Math.Sin(A) * F);
						E.X += Math.Cos(A) * 5f;
						E.Y += Math.Sin(A) * 5f;
						Dungeon.Area.Add(E);
					}
				} 

				this.PlaySfx("explosion");
				this.Done = true;
				Explosion.Make(this.X + W / 2, this.Y + H / 2, !this.Small);
				bool Fire = false;
				bool Ice = false;

				foreach (Buff Add in ToApply) {
					if (Add is BurningBuff) {
						Fire = true;
					} else if (Add is FrozenBuff) {
						Ice = true;
					} 
				}

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

							try {
								foreach (Buff Buff in ToApply) {
									Creature.AddBuff(Buff.GetClass().NewInstance().SetDuration(Buff.GetDuration()));
								}
							} catch (IllegalAccessException | InstantiationException) {
								E.PrintStackTrace();
							}
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

				int S = 3;

				if (!Fire && !Ice) {
					for (int Yy = -S; Yy <= S; Yy++) {
						for (int Xx = -S; Xx <= S; Xx++) {
							int X = (int) ((this.X + this.W / 2) / 16 + Xx);
							int Y = (int) ((this.Y + this.H / 2) / 16 + Yy);

							if (Math.Sqrt(Xx * Xx + Yy * Yy) <= S - 1) {
								int T = Dungeon.Level.Get(X, Y);

								if (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C || T == Terrain.FLOOR_D) {
									Dungeon.Level.Set(X, Y, Terrain.DIRT);
									Dungeon.Level.TileRegion(X, Y);
								} 
							} 
						}
					}
				} 

				for (int Yy = -S; Yy <= S; Yy++) {
					for (int Xx = -S; Xx <= S; Xx++) {
						if (Math.Sqrt(Xx * Xx + Yy * Yy) <= S) {
							int X = (int) ((this.X + this.W / 2) / 16 + Xx);
							int Y = (int) ((this.Y + this.H / 2) / 16 + Yy);

							if (Fire) {
								Dungeon.Level.SetOnFire(Level.ToIndex(X, Y), true);
							} 

							if (Ice) {
								Dungeon.Level.Freeze(Level.ToIndex(X, Y));
							} 
						} 
					}
				}

				bool Set = false;

				foreach (Room Room in Dungeon.Level.GetRooms()) {
					if (Room.Hidden) {
						if (Check(Room)) {
							Set = true;
						} 
					} 
				}

				if (Set) {
					Dungeon.Level.LoadPassable();
					Dungeon.Level.AddPhysics();
				} 
			} 

			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
		}

		public bool Check(Room Room) {
			for (int X = Room.Left; X <= Room.Right; X++) {
				for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
					if (Dungeon.Level.Get(X, Y) == Terrain.CRACK && this.GetDistanceTo(X * 16 + W / 2, Y * 16 + H / 2) <= 32f) {
						Make(Room);
						Achievements.Unlock(Achievements.FIND_SECRET_ROOM);
						int Num = GlobalSave.GetInt("num_secret_rooms_found") + 1;
						GlobalSave.Put("num_secret_rooms_found", Num);

						if (Num >= 3) {
							Achievements.Unlock(Achievements.UNLOCK_SPECTACLES);
						} 

						Room.Hidden = false;
						Dungeon.Level.Set(X, Y, Terrain.FLOOR_A);

						return true;
					} 
				}
			}

			return false;
		}

		public static Void Make(Room Room) {
			Player.Instance.PlaySfx("secret");
			Player.Instance.PlaySfx("secret_room");

			for (int X = Room.Left; X <= Room.Right; X++) {
				for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
					if (Dungeon.Level.IsValid(X, Y)) {
						Dungeon.Level.Set(X, Y, (byte) -Dungeon.Level.Data[Level.ToIndex(X, Y)]);
					} 
				}
			}
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H, (float) (Math.Cos(this.T * 16 + Math.PI)) * 3);
		}

		public override Void Render() {
			float Sx = (float) (Math.Cos(this.T * 16) / 2f) + 1;
			float Sy = (float) (Math.Cos(this.T * 16 + Math.PI) / 3f) + 1;

			if (Math.Cos(this.T * 16 + Math.PI) > 0) {
				Graphics.Batch.End();
				Gdx.Gl.GlEnable(GL20.GL_BLEND);
				Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
				Graphics.Shape.SetColor(1, 0, 0, 0.4f);
				Graphics.Shape.Begin(ShapeRenderer.ShapeType.Line);
				Graphics.Shape.Circle(this.X + W / 2, this.Y + H / 2, 24);
				Graphics.Shape.Circle(this.X + W / 2, this.Y + H / 2, 23);
				Graphics.Shape.Circle(this.X + W / 2, this.Y + H / 2, 23.5f);
				Graphics.EndAlphaShape();
			} 

			this.Animation.Render(this.X, this.Y, false, false, 5, 0, 0, this.Fliped ? -Sx : Sx, Sy);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null) {
				return true;
			} 

			if (Entity != null && !((Entity is Player && !((Player) Entity).IsRolling()) || Entity is BombEntity || Entity is SolidProp)) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}
