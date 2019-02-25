using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.mob.boss;
using BurningKnight.core.entity.creature.npc;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item.entity;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.special;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Door : SaveableEntity {
		protected void _Init() {
			{
				Depth = -1;
				AlwaysActive = true;
			}
		}

		private bool Vertical;
		private Body Body;
		private int NumCollisions;
		private static Animation VertAnimation = Animation.Make("actor-door-vertical", "-wooden");
		private static Animation HorizAnimation = Animation.Make("actor-door-horizontal", "-wooden");
		private static Animation VertAnimationFire = Animation.Make("actor-door-vertical", "-iron");
		private static Animation HorizAnimationFire = Animation.Make("actor-door-horizontal", "-iron");
		private static Animation IronLockAnimation = Animation.Make("door-lock", "-iron");
		private static Animation BronzeLockAnimation = Animation.Make("door-lock", "-bronze");
		public static Animation GoldLockAnimation = Animation.Make("door-lock", "-gold");
		private static Animation FireLockAnimation = Animation.Make("door-lock", "-fire");
		private AnimationData Animation;
		public AnimationData Locked;
		private AnimationData Unlock;
		private AnimationData Lk;
		private AnimationData LockAnim;
		public bool AutoLock;
		public bool Lockable;
		public bool Lock;
		public Class Key;
		private TextureRegion KeyRegion;
		private int Sx;
		private int Sy;
		private bool CollidingWithPlayer;
		private float Al;
		public static List<Door> All = new List<>();
		private float LastFlame;
		private Body Sensor;
		public bool BkDoor;
		private float Damage;
		public bool Burning;
		private float NoColl = -1;
		private float ClearT;
		private float Vt;

		public Door() {
			_Init();
		}

		public Animation GetAnimation() {
			if (this.Key == BurningKey.GetType()) {
				return FireLockAnimation;
			} 

			if (this.Key == KeyA.GetType()) {
				return BronzeLockAnimation;
			} 

			if (this.AutoLock || this.Key == KeyB.GetType()) {
				return IronLockAnimation;
			} 

			return GoldLockAnimation;
		}

		public override Void Init() {
			base.Init();
			All.Add(this);
		}

		public Door(int X, int Y, bool Vertical) {
			_Init();
			this.X = X * 16;
			this.Y = Y * 16;
			this.Sx = X;
			this.Sy = Y;
			this.Vertical = Vertical;
		}

		private Void SetPas(bool Pas) {
			Dungeon.Level.SetPassable((int) Math.Floor(this.X / 16), (int) Math.Floor((this.Y + 8) / 16), Pas);
		}

		public override Void Update(float Dt) {
			if (!this.Vertical) {
				this.Y = Sy * 16 - 8;
				this.X = Sx * 16;
			} else {
				this.Y = Sy * 16;
				this.X = Sx * 16 + 4;
			}


			if (this.Animation == null) {
				if (!this.Vertical) {
					this.Animation = BkDoor ? VertAnimationFire.Get("idle") : VertAnimation.Get("idle");
				} else {
					this.Animation = BkDoor ? HorizAnimationFire.Get("idle") : HorizAnimation.Get("idle");
				}


				this.Animation.SetAutoPause(true);
				this.Animation.SetPaused(true);
			} 

			if (this.Sensor == null) {
				this.Sensor = World.CreateSimpleBody(this, this.Vertical ? 1 : -1, this.Vertical ? -5 : 7, this.Vertical ? 6 : 18, this.Vertical ? 22 : 6, BodyDef.BodyType.DynamicBody, false);
				this.Sensor.SetSleepingAllowed(false);
				MassData Data = new MassData();
				Data.Mass = 1000000000000000f;
				this.Sensor.SetMassData(Data);
			} 

			World.CheckLocked(this.Sensor).SetTransform(this.X, this.Y, 0);

			if (Locked == null) {
				Animation Animation = GetAnimation();
				Locked = Animation.Get("idle");
				Unlock = Animation.Get("open");
				Lk = Animation.Get("close");
			} 

			if (this.NoColl > 0) {
				this.NoColl -= Dt;

				if (this.NoColl <= 0) {
					this.NoColl = -1;
					this.CollidingWithPlayer = false;
				} 
			} 

			if (this.CollidingWithPlayer) {
				if (Player.Instance.IsRolling()) {
					this.CollidingWithPlayer = false;
					OnCollisionEnd(Player.Instance);
				} 
			} 

			bool Last = this.Lock;

			if (this.Lock) {
				Vt = Math.Max(0, Vt - Dt);
			} 

			if (Dungeon.Depth == -2) {
				Room A = Dungeon.Level.FindRoomFor(this.X + (Vertical ? 16 : 0), this.Y + (Vertical ? 0 : 16));
				Room B = Dungeon.Level.FindRoomFor(this.X - (Vertical ? 16 : 0), this.Y - (Vertical ? 0 : 16));
				bool Found = false;

				foreach (Trader Trader in Trader.All) {
					if (Trader.Room == A || Trader.Room == B) {
						Found = true;

						break;
					} 
				}

				Lock = !Found;
			} else if (this.AutoLock) {
				this.Lock = Player.Instance != null && Player.Instance.Room != null && Player.Instance.Room.NumEnemies > 0 && !Player.Instance.HasBkKey;
			} 

			this.SetPas(false);

			if (this.Lock && !Last) {
				this.PlaySfx("door_lock");

				if (this.Body != null) {
					this.Body.GetFixtureList().Get(0).SetSensor(false);
				} 

				this.LockAnim = this.Lk;
				this.Animation.SetBack(true);
				this.Animation.SetPaused(false);
			} else if (!this.Lock && Last) {
				this.PlaySfx("door_unlock");
				this.LockAnim = this.Unlock;
			} 

			this.Al += ((this.CollidingWithPlayer ? 1 : 0) - this.Al) * Dt * 10;

			if (this.Lock && this.OnScreen && this.Al >= 0.5f && Input.Instance.WasPressed("interact")) {
				if ((this.Key == KeyC.GetType() && Player.Instance.GetKeys() > 0) || Player.Instance.GetInventory().Find(this.Key)) {
					if (this.Key == KeyC.GetType()) {
						Player.Instance.SetKeys(Player.Instance.GetKeys() - 1);
					} else {
						Player.Instance.GetInventory().Remove(BurningKey.GetType());
					}


					Player.Instance.PlaySfx("unlock");
					int Num = GlobalSave.GetInt("num_keys_used");
					GlobalSave.Put("num_keys_used", Num);

					if (Num >= 10) {
						Achievements.Unlock(Achievements.UNLOCK_LOOTPICK);
					} 

					this.Body = World.RemoveBody(this.Body);
					this.Lock = false;
					this.Animation.SetBack(false);
					this.Animation.SetPaused(false);
					this.LockAnim = this.Unlock;

					if (this.Key == KeyC.GetType()) {
						Room A = Dungeon.Level.FindRoomFor(this.X + (Vertical ? 16 : 0), this.Y + (Vertical ? 0 : 16));
						Room B = Dungeon.Level.FindRoomFor(this.X - (Vertical ? 16 : 0), this.Y - (Vertical ? 0 : 16));

						foreach (Trader Trader in Trader.All) {
							if (Trader.Room == A || Trader.Room == B) {
								Trader.Saved = true;
								GlobalSave.Put("npc_" + Trader.Id + "_saved", true);
								Trader.Become("thanks");

								if (Trader.Id != null && Trader.Id.Equals(NpcSaveRoom.SaveOrder[NpcSaveRoom.SaveOrder.Length - 1])) {
									Achievements.Unlock(Achievements.SAVE_ALL);
									GlobalSave.Put("all_npcs_saved", true);
								} 

								break;
							} 
						}
					} 
				} else {
					Vt = 1;
					this.PlaySfx("item_nocash");
					Camera.Shake(3);
				}

			} 

			if (this.NumCollisions == 0 && this.Animation.IsPaused() && this.Animation.GetFrame() == 3) {
				this.ClearT += Dt;

				if (this.ClearT > 0.5f) {
					this.Animation.SetBack(true);
					this.Animation.SetPaused(false);
					this.PlaySfx("door");
				} 
			} 

			if (this.Body == null && Lock) {
				this.Body = World.CreateSimpleBody(null, this.Vertical ? 2 : 0, this.Vertical ? -4 : 8, this.Vertical ? 4 : 16, this.Vertical ? 20 : 4, BodyDef.BodyType.DynamicBody, false);

				if (this.Body != null) {
					World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
				} 

				MassData Data = new MassData();
				Data.Mass = 1000000000000000f;

				if (this.Body != null) {
					this.Body.SetMassData(Data);
				} 
			} 

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 

			base.Update(Dt);

			if (this.Animation.Update(Dt)) {
				if (this.Animation.GetFrame() == 3) {
					this.Animation.SetPaused(true);
				} 

				if (this.NumCollisions == 0 && Animation.GetFrame() == 0) {
					Filter D = this.Sensor.GetFixtureList().Get(0).GetFilterData();
					D.MaskBits = 0x0003;
					this.Sensor.GetFixtureList().Get(0).SetFilterData(D);
				} 
			} 

			if (this.LockAnim != null) {
				if (this.LockAnim.Update(Dt)) {
					if (this.LockAnim == this.Unlock) {
						this.Lock = false;
						this.LockAnim = null;
					} else if (this.LockAnim == this.Lk) {
						this.LockAnim = this.Locked;
					} 
				} 
			} 

			if (!this.Burning) {
				if (Dungeon.Depth != -3) {
					int I = Level.ToIndex((int) Math.Floor(this.X / 16), (int) Math.Floor((this.Y + 8) / 16));
					int Info = Dungeon.Level.GetInfo(I);

					if (BitHelper.IsBitSet(Info, 0)) {
						this.Damage = 0;
						this.Burning = true;

						foreach (int J in PathFinder.NEIGHBOURS4) {
							Dungeon.Level.SetOnFire(I + J, true);
						}
					} 
				} 
			} else {
				if (Dungeon.Depth == -3) {
					Burning = false;

					return;
				} 

				InGameState.Burning = true;

				if (this.Key == KeyA.GetType() || this.Key == BurningKey.GetType()) {
					this.Burning = false;

					return;
				} 

				LastFlame += Dt;

				if (this.LastFlame >= 0.05f) {
					this.LastFlame = 0;
					TerrainFlameFx Fx = new TerrainFlameFx();
					Fx.X = this.X + Random.NewFloat(this.W);
					Fx.Y = this.Y + Random.NewFloat(this.H) - 4;
					Fx.Depth = 1;
					Dungeon.Area.Add(Fx);
				} 

				Damage += Dt / 5;

				if (Damage >= 1f) {
					if (this.Key == KeyA.GetType()) {
						Room Room = Dungeon.Level.FindRoomFor(this.X, this.Y);

						foreach (Trader Trader in Trader.All) {
							if (Trader.Room == Room) {
								Trader.Saved = true;
								GlobalSave.Put("npc_" + Trader.Id + "_saved", true);
								Trader.Become("thanks");

								if (Trader.Id.Equals(NpcSaveRoom.SaveOrder[NpcSaveRoom.SaveOrder.Length - 1])) {
									Achievements.Unlock(Achievements.SAVE_ALL);
									GlobalSave.Put("all_npcs_saved", true);
								} 

								break;
							} 
						}
					} 

					this.Done = true;

					for (int I = 0; I < 5; I++) {
						PoofFx Fx = new PoofFx();
						Fx.X = this.X + this.W / 2;
						Fx.Y = this.Y + this.H / 2;
						Dungeon.Area.Add(Fx);
					}
				} 
			}

		}

		public bool IsOpen() {
			return this.Animation.GetFrame() != 0;
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
			this.Sensor = World.RemoveBody(this.Sensor);
			All.Remove(this);
		}

		public override Void OnCollision(Entity Entity) {
			if (Entity is Creature && !(Entity is Boss)) {
				if (((Creature) Entity).HasBuff(Buffs.BURNING)) {
					this.Burning = true;
				} else if (this.Burning) {
					((Creature) Entity).AddBuff(new BurningBuff());
				} 

				if (this.Lock && this.Lockable && Entity is Player) {
					this.CollidingWithPlayer = true;
				} 

				if (this.Lock || !(Entity is Player)) {
					return;
				} 

				if (!this.IsOpen()) {
					if (this.Body != null) {
						this.Body.GetFixtureList().Get(0).SetSensor(true);
					} 

					this.PlaySfx("door");
				} 

				this.NumCollisions += 1;

				if (this.Sensor != null) {
					Filter D = this.Sensor.GetFixtureList().Get(0).GetFilterData();
					D.MaskBits = 0x0002;
					this.Sensor.GetFixtureList().Get(0).SetFilterData(D);
				} 

				this.Animation.SetBack(false);
				this.Animation.SetPaused(false);
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			if (Entity is Creature && !((Creature) Entity).IsFlying()) {
				if (this.Lock) {
					if (Entity is Player) {
						this.NoColl = 0.1f;
					} 

					return;
				} 

				if (Entity is Player) {
					this.NumCollisions = 0;
					this.ClearT = 0;
				} 
			} 
		}

		public Void RenderSigns() {
			if (LockAnim != null && KeyRegion != null && Al > 0) {
				float V = Vt <= 0 ? 0 : (float) (Math.Cos(Dungeon.Time * 18f) * 5 * (Vt));
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(KeyRegion, this.X - 3 + (16 - this.KeyRegion.GetRegionWidth()) / 2 + V + (this.Vertical ? 0f : 4f), this.Y + 16);
				Graphics.Batch.SetColor(1, 1, 1, 1);
			} 
		}

		public override Void Render() {
			if (Animation == null) {
				return;
			} 

			if (this.Key != null && this.Key != KeyB.GetType() && this.KeyRegion == null) {
				try {
					Key Key = (Key) this.Key.NewInstance();
					this.KeyRegion = Key.GetSprite();
				} catch (InstantiationException) {
					E.PrintStackTrace();
				} catch (IllegalAccessException) {
					E.PrintStackTrace();
				}
			} 

			if (this.Lock && this.LockAnim == null) {
				this.LockAnim = this.Locked;
				this.SetPas(false);
			} 

			this.Animation.Render(this.X, this.Y, false);

			if (this.LockAnim != null) {
				if (this.Al > 0) {
					Graphics.Batch.End();
					Mob.Shader.Begin();
					Mob.Shader.SetUniformf("u_color", ColorUtils.WHITE);
					Mob.Shader.SetUniformf("u_a", Al);
					Mob.Shader.End();
					Graphics.Batch.SetShader(Mob.Shader);
					Graphics.Batch.Begin();

					for (int Yy = -1; Yy < 2; Yy++) {
						for (int Xx = -1; Xx < 2; Xx++) {
							if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
								this.LockAnim.Render(this.X + (this.Vertical ? -1 : 3) + Xx, this.Y + (this.Vertical ? 2 : -2) + Yy, false);
							} 
						}
					}

					Graphics.Batch.End();
					Graphics.Batch.SetShader(null);
					Graphics.Batch.Begin();
				} 

				this.LockAnim.Render(this.X + (this.Vertical ? -1 : 3), this.Y + (this.Vertical ? 2 : -2), false);
			} 
		}

		public override Void RenderShadow() {
			if (this.Animation == null) {
				return;
			} 

			Graphics.Shape.End();
			Graphics.Batch.Begin();
			this.Animation.Render(this.X, this.Y - (this.Vertical ? H / 2 - 2 : H), false, true, this.Animation.GetFrame());
			Graphics.Batch.End();
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Burning = Reader.ReadBoolean();

			if (this.Burning) {
				this.Damage = Reader.ReadFloat();
			} 

			this.Vertical = Reader.ReadBoolean();

			if (!this.Vertical) {
				this.Animation = VertAnimation.Get("idle");
			} else {
				this.Animation = HorizAnimation.Get("idle");
			}


			this.Animation.SetPaused(true);
			this.Animation.SetAutoPause(true);
			this.AutoLock = Reader.ReadBoolean();
			this.Lock = Reader.ReadBoolean();
			this.Lockable = Reader.ReadBoolean();

			if (Reader.ReadBoolean()) {
				try {
					this.Key = Class.ForName(Reader.ReadString());
				} catch (ClassNotFoundException) {
					E.PrintStackTrace();
				}
			} 

			this.Sx = Reader.ReadInt16();
			this.Sy = Reader.ReadInt16();
			this.BkDoor = Reader.ReadBoolean();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Burning);

			if (this.Burning) {
				Writer.WriteFloat(this.Damage);
			} 

			Writer.WriteBoolean(this.Vertical);
			Writer.WriteBoolean(this.AutoLock);
			Writer.WriteBoolean(this.Lock);
			Writer.WriteBoolean(this.Lockable);
			Writer.WriteBoolean(this.Lock && this.Key != null);

			if (this.Lock && this.Key != null) {
				Writer.WriteString(this.Key.GetName());
			} 

			Writer.WriteInt16((short) this.Sx);
			Writer.WriteInt16((short) this.Sy);
			Writer.WriteBoolean(this.BkDoor);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is RollingSpike || ((Entity is Mob || Entity is BombEntity))) {
				return true;
			} 

			if ((Entity is Creature || Entity is PetEntity)) {
				if (!this.Lock || Entity is PetEntity) {
					return false;
				} 
			} 

			return false;
		}
	}
}
