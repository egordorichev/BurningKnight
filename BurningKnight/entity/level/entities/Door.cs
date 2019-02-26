using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.item.entity;
using BurningKnight.entity.item.key;
using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.level.save;
using BurningKnight.entity.trap;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Door : SaveableEntity {
		private static Animation VertAnimation = Animation.Make("actor-door-vertical", "-wooden");
		private static Animation HorizAnimation = Animation.Make("actor-door-horizontal", "-wooden");
		private static Animation VertAnimationFire = Animation.Make("actor-door-vertical", "-iron");
		private static Animation HorizAnimationFire = Animation.Make("actor-door-horizontal", "-iron");
		private static Animation IronLockAnimation = Animation.Make("door-lock", "-iron");
		private static Animation BronzeLockAnimation = Animation.Make("door-lock", "-bronze");
		public static Animation GoldLockAnimation = Animation.Make("door-lock", "-gold");
		private static Animation FireLockAnimation = Animation.Make("door-lock", "-fire");
		public static List<Door> All = new List<>();
		private float Al;
		private AnimationData Animation;
		public bool AutoLock;
		public bool BkDoor;
		private Body Body;
		public bool Burning;
		private float ClearT;
		private bool CollidingWithPlayer;
		private float Damage;
		public Class Key;
		private TextureRegion KeyRegion;
		private float LastFlame;
		private AnimationData Lk;
		public bool Lock;
		public bool Lockable;
		private AnimationData LockAnim;
		public AnimationData Locked;
		private float NoColl = -1;
		private int NumCollisions;
		private Body Sensor;
		private int Sx;
		private int Sy;
		private AnimationData Unlock;

		private bool Vertical;
		private float Vt;

		public Door() {
			_Init();
		}

		public Door(int X, int Y, bool Vertical) {
			_Init();
			this.X = X * 16;
			this.Y = Y * 16;
			Sx = X;
			Sy = Y;
			this.Vertical = Vertical;
		}

		protected void _Init() {
			{
				Depth = -1;
				AlwaysActive = true;
			}
		}

		public Animation GetAnimation() {
			if (Key == BurningKey.GetType()) return FireLockAnimation;

			if (Key == KeyA.GetType()) return BronzeLockAnimation;

			if (AutoLock || Key == KeyB.GetType()) return IronLockAnimation;

			return GoldLockAnimation;
		}

		public override void Init() {
			base.Init();
			All.Add(this);
		}

		private void SetPas(bool Pas) {
			Dungeon.Level.SetPassable((int) Math.Floor(this.X / 16), (int) Math.Floor((this.Y + 8) / 16), Pas);
		}

		public override void Update(float Dt) {
			if (!Vertical) {
				this.Y = Sy * 16 - 8;
				this.X = Sx * 16;
			}
			else {
				this.Y = Sy * 16;
				this.X = Sx * 16 + 4;
			}


			if (this.Animation == null) {
				if (!Vertical)
					Animation = BkDoor ? VertAnimationFire.Get("idle") : VertAnimation.Get("idle");
				else
					Animation = BkDoor ? HorizAnimationFire.Get("idle") : HorizAnimation.Get("idle");


				Animation.SetAutoPause(true);
				Animation.SetPaused(true);
			}

			if (Sensor == null) {
				Sensor = World.CreateSimpleBody(this, Vertical ? 1 : -1, Vertical ? -5 : 7, Vertical ? 6 : 18, Vertical ? 22 : 6, BodyDef.BodyType.DynamicBody, false);
				Sensor.SetSleepingAllowed(false);
				MassData Data = new MassData();
				Data.Mass = 1000000000000000f;
				Sensor.SetMassData(Data);
			}

			World.CheckLocked(Sensor).SetTransform(this.X, this.Y, 0);

			if (Locked == null) {
				var Animation = GetAnimation();
				Locked = Animation.Get("idle");
				Unlock = Animation.Get("open");
				Lk = Animation.Get("close");
			}

			if (NoColl > 0) {
				NoColl -= Dt;

				if (NoColl <= 0) {
					NoColl = -1;
					CollidingWithPlayer = false;
				}
			}

			if (CollidingWithPlayer)
				if (Player.Instance.IsRolling()) {
					CollidingWithPlayer = false;
					OnCollisionEnd(Player.Instance);
				}

			var Last = Lock;

			if (Lock) Vt = Math.Max(0, Vt - Dt);

			if (Dungeon.Depth == -2) {
				Room A = Dungeon.Level.FindRoomFor(this.X + (Vertical ? 16 : 0), this.Y + (Vertical ? 0 : 16));
				Room B = Dungeon.Level.FindRoomFor(this.X - (Vertical ? 16 : 0), this.Y - (Vertical ? 0 : 16));
				var Found = false;

				foreach (Trader Trader in Trader.All)
					if (Trader.Room == A || Trader.Room == B) {
						Found = true;

						break;
					}

				Lock = !Found;
			}
			else if (AutoLock) {
				Lock = Player.Instance != null && Player.Instance.Room != null && Player.Instance.Room.NumEnemies > 0 && !Player.Instance.HasBkKey;
			}

			SetPas(false);

			if (Lock && !Last) {
				PlaySfx("door_lock");

				if (Body != null) Body.GetFixtureList().Get(0).SetSensor(false);

				LockAnim = Lk;
				Animation.SetBack(true);
				Animation.SetPaused(false);
			}
			else if (!Lock && Last) {
				PlaySfx("door_unlock");
				LockAnim = Unlock;
			}

			Al += ((CollidingWithPlayer ? 1 : 0) - Al) * Dt * 10;

			if (Lock && OnScreen && Al >= 0.5f && Input.Instance.WasPressed("interact")) {
				if (Key == KeyC.GetType() && Player.Instance.GetKeys() > 0 || Player.Instance.GetInventory().Find(Key)) {
					if (Key == KeyC.GetType())
						Player.Instance.SetKeys(Player.Instance.GetKeys() - 1);
					else
						Player.Instance.GetInventory().Remove(BurningKey.GetType());


					Player.Instance.PlaySfx("unlock");
					var Num = GlobalSave.GetInt("num_keys_used");
					GlobalSave.Put("num_keys_used", Num);

					if (Num >= 10) Achievements.Unlock(Achievements.UNLOCK_LOOTPICK);

					Body = World.RemoveBody(Body);
					Lock = false;
					Animation.SetBack(false);
					Animation.SetPaused(false);
					LockAnim = Unlock;

					if (Key == KeyC.GetType()) {
						Room A = Dungeon.Level.FindRoomFor(this.X + (Vertical ? 16 : 0), this.Y + (Vertical ? 0 : 16));
						Room B = Dungeon.Level.FindRoomFor(this.X - (Vertical ? 16 : 0), this.Y - (Vertical ? 0 : 16));

						foreach (Trader Trader in Trader.All)
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
				else {
					Vt = 1;
					PlaySfx("item_nocash");
					Camera.Shake(3);
				}
			}

			if (NumCollisions == 0 && this.Animation.IsPaused() && this.Animation.GetFrame() == 3) {
				ClearT += Dt;

				if (ClearT > 0.5f) {
					Animation.SetBack(true);
					Animation.SetPaused(false);
					PlaySfx("door");
				}
			}

			if (Body == null && Lock) {
				Body = World.CreateSimpleBody(null, Vertical ? 2 : 0, Vertical ? -4 : 8, Vertical ? 4 : 16, Vertical ? 20 : 4, BodyDef.BodyType.DynamicBody, false);

				if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);

				MassData Data = new MassData();
				Data.Mass = 1000000000000000f;

				if (Body != null) Body.SetMassData(Data);
			}

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);

			base.Update(Dt);

			if (this.Animation.Update(Dt)) {
				if (Animation.GetFrame() == 3) Animation.SetPaused(true);

				if (NumCollisions == 0 && Animation.GetFrame() == 0) {
					Filter D = Sensor.GetFixtureList().Get(0).GetFilterData();
					D.MaskBits = 0x0003;
					Sensor.GetFixtureList().Get(0).SetFilterData(D);
				}
			}

			if (LockAnim != null)
				if (LockAnim.Update(Dt)) {
					if (LockAnim == Unlock) {
						Lock = false;
						LockAnim = null;
					}
					else if (LockAnim == Lk) {
						LockAnim = Locked;
					}
				}

			if (!Burning) {
				if (Dungeon.Depth != -3) {
					int I = Level.ToIndex((int) Math.Floor(this.X / 16), (int) Math.Floor((this.Y + 8) / 16));
					int Info = Dungeon.Level.GetInfo(I);

					if (BitHelper.IsBitSet(Info, 0)) {
						Damage = 0;
						Burning = true;

						foreach (var J in PathFinder.NEIGHBOURS4) Dungeon.Level.SetOnFire(I + J, true);
					}
				}
			}
			else {
				if (Dungeon.Depth == -3) {
					Burning = false;

					return;
				}

				InGameState.Burning = true;

				if (Key == KeyA.GetType() || Key == BurningKey.GetType()) {
					Burning = false;

					return;
				}

				LastFlame += Dt;

				if (LastFlame >= 0.05f) {
					LastFlame = 0;
					var Fx = new TerrainFlameFx();
					Fx.X = this.X + Random.NewFloat(W);
					Fx.Y = this.Y + Random.NewFloat(H) - 4;
					Fx.Depth = 1;
					Dungeon.Area.Add(Fx);
				}

				Damage += Dt / 5;

				if (Damage >= 1f) {
					if (Key == KeyA.GetType()) {
						Room Room = Dungeon.Level.FindRoomFor(this.X, this.Y);

						foreach (Trader Trader in Trader.All)
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

					Done = true;

					for (var I = 0; I < 5; I++) {
						var Fx = new PoofFx();
						Fx.X = this.X + W / 2;
						Fx.Y = this.Y + H / 2;
						Dungeon.Area.Add(Fx);
					}
				}
			}
		}

		public bool IsOpen() {
			return Animation.GetFrame() != 0;
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
			Sensor = World.RemoveBody(Sensor);
			All.Remove(this);
		}

		public override void OnCollision(Entity Entity) {
			if (Entity is Creature && !(Entity is Boss)) {
				if (((Creature) Entity).HasBuff(Buffs.BURNING))
					Burning = true;
				else if (Burning) ((Creature) Entity).AddBuff(new BurningBuff());

				if (Lock && Lockable && Entity is Player) CollidingWithPlayer = true;

				if (Lock || !(Entity is Player)) return;

				if (!IsOpen()) {
					if (Body != null) Body.GetFixtureList().Get(0).SetSensor(true);

					PlaySfx("door");
				}

				NumCollisions += 1;

				if (Sensor != null) {
					Filter D = Sensor.GetFixtureList().Get(0).GetFilterData();
					D.MaskBits = 0x0002;
					Sensor.GetFixtureList().Get(0).SetFilterData(D);
				}

				Animation.SetBack(false);
				Animation.SetPaused(false);
			}
		}

		public override void OnCollisionEnd(Entity Entity) {
			if (Entity is Creature && !((Creature) Entity).IsFlying()) {
				if (Lock) {
					if (Entity is Player) NoColl = 0.1f;

					return;
				}

				if (Entity is Player) {
					NumCollisions = 0;
					ClearT = 0;
				}
			}
		}

		public void RenderSigns() {
			if (LockAnim != null && KeyRegion != null && Al > 0) {
				var V = Vt <= 0 ? 0 : Math.Cos(Dungeon.Time * 18f) * 5 * Vt;
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(KeyRegion, this.X - 3 + (16 - KeyRegion.GetRegionWidth()) / 2 + V + (Vertical ? 0f : 4f), this.Y + 16);
				Graphics.Batch.SetColor(1, 1, 1, 1);
			}
		}

		public override void Render() {
			if (Animation == null) return;

			if (this.Key != null && this.Key != KeyB.GetType() && KeyRegion == null)
				try {
					var Key = (Key) this.Key.NewInstance();
					KeyRegion = Key.GetSprite();
				}
				catch (InstantiationException) {
					E.PrintStackTrace();
				}
				catch (IllegalAccessException) {
					E.PrintStackTrace();
				}

			if (Lock && LockAnim == null) {
				LockAnim = Locked;
				SetPas(false);
			}

			Animation.Render(this.X, this.Y, false);

			if (LockAnim != null) {
				if (Al > 0) {
					Graphics.Batch.End();
					Mob.Shader.Begin();
					Mob.Shader.SetUniformf("u_color", ColorUtils.WHITE);
					Mob.Shader.SetUniformf("u_a", Al);
					Mob.Shader.End();
					Graphics.Batch.SetShader(Mob.Shader);
					Graphics.Batch.Begin();

					for (var Yy = -1; Yy < 2; Yy++)
					for (var Xx = -1; Xx < 2; Xx++)
						if (Math.Abs(Xx) + Math.Abs(Yy) == 1)
							LockAnim.Render(this.X + (Vertical ? -1 : 3) + Xx, this.Y + (Vertical ? 2 : -2) + Yy, false);

					Graphics.Batch.End();
					Graphics.Batch.SetShader(null);
					Graphics.Batch.Begin();
				}

				LockAnim.Render(this.X + (Vertical ? -1 : 3), this.Y + (Vertical ? 2 : -2), false);
			}
		}

		public override void RenderShadow() {
			if (Animation == null) return;

			Graphics.Shape.End();
			Graphics.Batch.Begin();
			Animation.Render(this.X, this.Y - (Vertical ? H / 2 - 2 : H), false, true, Animation.GetFrame());
			Graphics.Batch.End();
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Burning = Reader.ReadBoolean();

			if (Burning) Damage = Reader.ReadFloat();

			Vertical = Reader.ReadBoolean();

			if (!Vertical)
				Animation = VertAnimation.Get("idle");
			else
				Animation = HorizAnimation.Get("idle");


			Animation.SetPaused(true);
			Animation.SetAutoPause(true);
			AutoLock = Reader.ReadBoolean();
			Lock = Reader.ReadBoolean();
			Lockable = Reader.ReadBoolean();

			if (Reader.ReadBoolean())
				try {
					Key = Class.ForName(Reader.ReadString());
				}
				catch (ClassNotFoundException) {
					E.PrintStackTrace();
				}

			Sx = Reader.ReadInt16();
			Sy = Reader.ReadInt16();
			BkDoor = Reader.ReadBoolean();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Burning);

			if (Burning) Writer.WriteFloat(Damage);

			Writer.WriteBoolean(Vertical);
			Writer.WriteBoolean(AutoLock);
			Writer.WriteBoolean(Lock);
			Writer.WriteBoolean(Lockable);
			Writer.WriteBoolean(Lock && Key != null);

			if (Lock && Key != null) Writer.WriteString(Key.GetName());

			Writer.WriteInt16((short) Sx);
			Writer.WriteInt16((short) Sy);
			Writer.WriteBoolean(BkDoor);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is RollingSpike || Entity is Mob || Entity is BombEntity) return true;

			if (Entity is Creature || Entity is PetEntity)
				if (!Lock || Entity is PetEntity)
					return false;

			return false;
		}
	}
}