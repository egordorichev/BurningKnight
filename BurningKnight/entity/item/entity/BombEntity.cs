using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.creature.buff.fx;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.save;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.entity {
	public class BombEntity : Entity {
		public static Animation Normal = Animation.Make("actor-bomb", "-normal");
		public static Animation Iced = Animation.Make("actor-bomb", "-ice_bomb");
		public static Animation Poisoned = Animation.Make("actor-bomb", "-poison");
		public static Animation Tiny = Animation.Make("actor-bomb", "-small");
		private AnimationData Animation;
		private Body Body;
		public bool Bullets;
		private bool Burning;
		private bool Fliped;
		private bool Ice;
		private float LastFlame;
		public bool LeaveSmall;
		private PointLight Light;
		private float Mod;
		public Creature Owner;
		private bool Poison;
		public bool Small;
		private float T;
		public List<Buff> ToApply = new List<>();
		public Point Vel = new Point();

		public BombEntity(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			Fliped = Random.Chance(50);
			Light = World.NewLight(32, new Color(1, 0, 0, 1), 64, 0, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		protected void _Init() {
			{
				Depth = -1;
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Mod = Random.NewFloat(0.95f, 1.05f);

			foreach (Buff Buff in ToApply)
				if (Buff is BurningBuff)
					Burning = true;
				else if (Buff is PoisonedBuff)
					Poison = true;
				else if (Buff is FrozenBuff) Ice = true;

			if (Small) {
				W = 8;
				H = 8;
			}
			else {
				W = 10;
				H = 14;
			}


			if (Ice)
				Animation = Iced.Get("idle");
			else if (Poison)
				Animation = Poisoned.Get("idle");
			else if (Small)
				Animation = Tiny.Get("idle");
			else
				Animation = Normal.Get("idle");


			Body = World.CreateSimpleBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, false);
			MassData Data = new MassData();
			Data.Mass = 0.1f;
			Body.SetMassData(Data);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			PlaySfx("bomb_placed");
			Room Room = Dungeon.Level.FindRoomFor(this.X, this.Y);

			if (Shopkeeper.Instance != null && Shopkeeper.Instance.Room == Room) Shopkeeper.Instance.Enrage();
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public BombEntity ToMouseVel() {
			return VelTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y, 60f);
		}

		public BombEntity VelTo(float X, float Y, float F) {
			var A = (float) Math.Atan2(Y - this.Y - 8, X - this.X - 8);
			Vel = new Point((float) Math.Cos(A) * F, (float) Math.Sin(A) * F);
			this.X += Math.Cos(A) * 5f;
			this.Y += Math.Sin(A) * 5f;

			return this;
		}

		public override void Update(float Dt) {
			this.T += Dt;

			if (Burning) {
				LastFlame += Dt;

				if (LastFlame >= 0.1f) {
					LastFlame = 0;
					Dungeon.Area.Add(new FlameFx(this));
				}
			}

			this.X = Body.GetPosition().X;
			this.Y = Body.GetPosition().Y;
			Vel.Mul(0.95f);
			Body.SetLinearVelocity(Vel);

			if (Animation.Update(Dt * Mod)) {
				if (Bullets)
					for (var I = 0; I < 8; I++) {
						BulletProjectile Bullet = new NanoBullet();
						float F = 60;
						var A = I * (Math.PI / 4);
						Bullet.Damage = 10;
						Bullet.Bad = Owner is Mob;
						Bullet.X = (float) (this.X + Math.Cos(A) * 8);
						Bullet.Y = (float) (this.Y + Math.Sin(A) * 8);
						Bullet.Velocity.X = Math.Cos(A) * F;
						Bullet.Velocity.Y = Math.Sin(A) * F;
						Dungeon.Area.Add(Bullet);
					}

				if (LeaveSmall && !Small)
					for (var I = 0; I < 4; I++) {
						var E = new BombEntity(this.X + W - Random.NewFloat(W), this.Y + H - Random.NewFloat(H));
						E.Small = true;
						var A = (float) (I * Math.PI / 2);
						var F = 200f;
						E.Vel = new Point((float) Math.Cos(A) * F, (float) Math.Sin(A) * F);
						E.X += Math.Cos(A) * 5f;
						E.Y += Math.Sin(A) * 5f;
						Dungeon.Area.Add(E);
					}

				PlaySfx("explosion");
				Done = true;
				Explosion.Make(this.X + W / 2, this.Y + H / 2, !Small);
				var Fire = false;
				var Ice = false;

				foreach (Buff Add in ToApply)
					if (Add is BurningBuff)
						Fire = true;
					else if (Add is FrozenBuff) Ice = true;

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

							try {
								foreach (Buff Buff in ToApply) Creature.AddBuff(Buff.GetClass().NewInstance().SetDuration(Buff.GetDuration()));
							}
							catch (IllegalAccessException |

							InstantiationException) {
								E.PrintStackTrace();
							}
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

				var S = 3;

				if (!Fire && !Ice)
					for (var Yy = -S; Yy <= S; Yy++)
					for (var Xx = -S; Xx <= S; Xx++) {
						var X = (int) ((this.X + W / 2) / 16 + Xx);
						var Y = (int) ((this.Y + H / 2) / 16 + Yy);

						if (Math.Sqrt(Xx * Xx + Yy * Yy) <= S - 1) {
							int T = Dungeon.Level.Get(X, Y);

							if (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C || T == Terrain.FLOOR_D) {
								Dungeon.Level.Set(X, Y, Terrain.DIRT);
								Dungeon.Level.TileRegion(X, Y);
							}
						}
					}

				for (var Yy = -S; Yy <= S; Yy++)
				for (var Xx = -S; Xx <= S; Xx++)
					if (Math.Sqrt(Xx * Xx + Yy * Yy) <= S) {
						var X = (int) ((this.X + W / 2) / 16 + Xx);
						var Y = (int) ((this.Y + H / 2) / 16 + Yy);

						if (Fire) Dungeon.Level.SetOnFire(Level.ToIndex(X, Y), true);

						if (Ice) Dungeon.Level.Freeze(Level.ToIndex(X, Y));
					}

				var Set = false;

				foreach (Room Room in Dungeon.Level.GetRooms())
					if (Room.Hidden)
						if (Check(Room))
							Set = true;

				if (Set) {
					Dungeon.Level.LoadPassable();
					Dungeon.Level.AddPhysics();
				}
			}

			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
		}

		public bool Check(Room Room) {
			for (var X = Room.Left; X <= Room.Right; X++)
			for (var Y = Room.Top; Y <= Room.Bottom; Y++)
				if (Dungeon.Level.Get(X, Y) == Terrain.CRACK && GetDistanceTo(X * 16 + W / 2, Y * 16 + H / 2) <= 32f) {
					Make(Room);
					Achievements.Unlock(Achievements.FIND_SECRET_ROOM);
					var Num = GlobalSave.GetInt("num_secret_rooms_found") + 1;
					GlobalSave.Put("num_secret_rooms_found", Num);

					if (Num >= 3) Achievements.Unlock(Achievements.UNLOCK_SPECTACLES);

					Room.Hidden = false;
					Dungeon.Level.Set(X, Y, Terrain.FLOOR_A);

					return true;
				}

			return false;
		}

		public static void Make(Room Room) {
			Player.Instance.PlaySfx("secret");
			Player.Instance.PlaySfx("secret_room");

			for (var X = Room.Left; X <= Room.Right; X++)
			for (var Y = Room.Top; Y <= Room.Bottom; Y++)
				if (Dungeon.Level.IsValid(X, Y))
					Dungeon.Level.Set(X, Y, (byte) -Dungeon.Level.Data[Level.ToIndex(X, Y)]);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W, H, (float) Math.Cos(T * 16 + Math.PI) * 3);
		}

		public override void Render() {
			var Sx = Math.Cos(T * 16) / 2f + 1;
			var Sy = Math.Cos(T * 16 + Math.PI) / 3f + 1;

			if (Math.Cos(T * 16 + Math.PI) > 0) {
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

			Animation.Render(this.X, this.Y, false, false, 5, 0, 0, Fliped ? -Sx : Sx, Sy);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null) return true;

			if (Entity != null && !(Entity is Player && !((Player) Entity).IsRolling() || Entity is BombEntity || Entity is SolidProp)) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}