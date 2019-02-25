using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.creature.mob.desert;
using BurningKnight.core.entity.creature.mob.tech;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.entity.level.levels.ice;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature {
	public class Creature : SaveableEntity {
		public int Hw;
		public int Hh;
		public float Z;
		public float Lz;
		public float A = 1f;
		public Long LastIndex;
		public bool Invisible;
		public bool Penetrates;
		public bool ExplosionBlock;
		public bool Freezed;
		public bool Poisoned;
		public float Speed = 10f;
		public float MaxSpeed = 90f;
		public int Hx;
		public int Hy;
		public bool Remove;
		public int SlowLiquidResist = 0;
		public int IceResitant;
		public Point Knockback = new Point();
		public Room Room;
		protected bool Flying = false;
		protected int Hp;
		protected int HpMax;
		protected int Damage = 2;
		protected int Defense;
		protected float Invt = 0;
		protected bool Dead;
		protected bool Unhittable = false;
		protected Body Body;
		protected float Mul = 0.7f;
		protected float Timer;
		protected bool Flipped = false;
		protected Dictionary<string, Buff> Buffs = new Dictionary<>();
		protected float Invtt;
		protected bool ShouldDie = false;
		protected bool IgnorePos;
		protected Point Acceleration = new Point();
		protected bool[] Touches = new bool[Terrain.SIZE];
		protected bool IgnoreAcceleration;
		public float InvTime = 0.2f;
		public float KnockbackMod = 1f;

		public float GetWeaponAngle() {
			Point Aim = GetAim();

			return GetAngleTo(Aim.X, Aim.Y);
		}

		public Point GetAim() {
			return Input.Instance.WorldMouse;
		}

		public Body CreateSimpleBody(int X, int Y, int W, int H, BodyDef.BodyType Type, bool Sensor) {
			this.Hx = X;
			this.Hy = Y;
			this.Hw = W;
			this.Hh = H;
			Body Body = World.CreateSimpleBody(this, X, Y, W, H, Type, Sensor);

			if (Body != null) {
				Body.SetSleepingAllowed(false);
			} 

			return Body;
		}

		public Void ModifyDefense(int Amount) {
			this.Defense += Amount;
		}

		public Void Tp(float X, float Y) {
			this.X = X;
			this.Y = Y;

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0);
				this.Lz = this.Z;
			} 
		}

		public float GetInvt() {
			return this.Invt;
		}

		public Void SetInvt(float Invt) {
			this.Invtt = Invt;
		}

		public override Void Init() {
			base.Init();
			this.T = Random.NewFloat(1024);
			this.Hp = this.HpMax;
		}

		public override Void Destroy() {
			base.Destroy();

			if (this.Body != null) {
				this.Body = World.RemoveBody(this.Body);
			} 
		}

		public override Void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y, this.W, this.H, 6);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (this.IsFlying() && Entity is Level) {
				return false;
			} else if (Entity is Creature) {
				if (this.HasBuff(Buffs.BURNING)) {
					((Creature) Entity).AddBuff(new BurningBuff());
				} 

				if (!(this is Player && Entity is Mummy)) {
					return false;
				} 
			} else if (Entity is ItemHolder) {
				return false;
			} else if (Entity is Weapon && ((Weapon) Entity).GetOwner() == this) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public bool HasBuff(string Buff) {
			return this.Buffs.ContainsKey(Buff);
		}

		protected Void OnRoomChange() {

		}

		public Void DeathEffect(AnimationData Killed) {
			LevelSave.Remove(this);

			if (Settings.Gore) {
				foreach (Animation.Frame Frame in Killed.GetFrames()) {
					GoreFx Fx = new GoreFx();
					Fx.Texture = Frame.Frame;
					Fx.X = this.X + this.W / 2;
					Fx.Y = this.Y + this.H / 2;
					Dungeon.Area.Add(Fx);
				}
			} 

			if (Settings.Blood) {
				for (int I = 0; I < Random.NewInt(2, 3); I++) {
					BloodSplatFx Fxx = new BloodSplatFx();
					Fxx.SizeMod = 2;
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}

				for (int I = 0; I < Random.NewInt(2, 5); I++) {
					BloodSplatFx Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.A = Random.NewFloat(360);
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}

				for (int I = 0; I < Random.NewInt(3, 5); I++) {
					BloodSplatFx Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.Cntr = true;
					Fxx.A = Random.NewFloat(360);
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}
			} 

			this.Poof();
			BloodFx.Add(this, 20);
		}

		public Void Poof() {
			for (int I = 0; I < 10; I++) {
				PoofFx Fx = new PoofFx();
				Fx.X = this.X + this.W / 2;
				Fx.Y = this.Y + this.H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override Void Update(float Dt) {
			Arrays.Fill(Touches, false);

			for (int X = (int) Math.Floor((this.Hx + this.X) / 16); X < Math.Ceil((this.Hx + this.X + this.Hw) / 16); X++) {
				for (int Y = (int) Math.Floor((this.Hy + this.Y + 8) / 16); Y < Math.Ceil((this.Hy + this.Y + 8 + this.Hh / 3) / 16); Y++) {
					if (X < 0 || Y < 0 || X >= Level.GetWidth() || Y >= Level.GetHeight()) {
						continue;
					} 

					if (CollisionHelper.Check(this.Hx + this.X, this.Hy + this.Y, this.Hw, this.Hh / 3, X * 16, Y * 16 - 8, 16, 16)) {
						int I = Level.ToIndex(X, Y);
						int Info = Dungeon.Level.GetInfo(I);
						byte T = Dungeon.Level.Get(I);
						this.OnTouch(T, X, Y, Info);
						Touches[T] = true;
						byte L = Dungeon.Level.LiquidData[I];

						if (L > 0) {
							Rectangle R = Level.COLLISIONS[Dungeon.Level.LiquidVariants[I]];

							if (CollisionHelper.Check(this.Hx + this.X, this.Hy + this.Y, this.Hw, this.Hh / 3, X * 16 + R.X, Y * 16 - 8 + R.Y, R.Width, R.Height)) {
								Touches[L] = true;
								this.OnTouch(L, X, Y, Info);
							} 
						} 
					} 
				}
			}

			if (!IgnoreAcceleration) {
				this.Acceleration.X = 0;
				this.Acceleration.Y = 0;
			} 

			Buff[] Buffs = this.Buffs.Values().ToArray({});
			int Sx = (int) (this.X + this.W / 2);
			int Sy = (int) (this.Y + this.H / 2);

			for (int I = Buffs.Length - 1; I >= 0; I--) {
				if (Buffs[I] is BurningBuff) {
					InGameState.Burning = true;

					if (!(this is BurningMan) && !this.IsFlying()) {
						Dungeon.Level.SetOnFire(Level.ToIndex(Sx / 16, Sy / 16), true, false);
					} 
				} 

				Buffs[I].Update(Dt);
			}

			Room Room = Dungeon.Level.FindRoomFor(Sx, Sy);

			if (Room != this.Room) {
				this.Room = Room;
				this.OnRoomChange();
			} 

			base.Update(Dt);

			if (this.ShouldDie) {
				this.Die();
				this.ShouldDie = false;
			} 

			if (this.Hp == 0 && !this.Dead) {
				this.Die(true);
			} 

			if (this.Remove) {
				this.Destroy();
				this.Remove = false;
			} 

			if (this.Dead) {
				return;
			} 

			if (this.Freezed) {
				this.Invt = Math.Max(0, this.Invt - Dt);
				this.Invtt = Math.Max(0, this.Invtt - Dt);
				this.Velocity.X = 0;
				this.Velocity.Y = 0;
				this.Body.SetLinearVelocity(new Vector2(this.Velocity.X + this.Knockback.X, this.Velocity.Y + this.Knockback.Y));

				return;
			} 

			if (!this.IsFlying() && this.Touches[Terrain.WATER] && !IgnoreWater() && !(this is Tank) && (!(this is Player) || !((Player) this).IsRolling())) {
				this.Velocity.Y -= Dt * 300;
			} 

			if (this is Player && ((Player) this).IsRolling()) {
				this.Velocity.Mul(this.Mul);
			} else {
				this.Velocity.Mul((this.Touches[Terrain.COBWEB] && !this.IsFlying() ? 0.5f : (IceResitant == 0 && this.Touches[Terrain.ICE] && !this.IsFlying() ? 0.95f : this.Mul)));
			}


			this.Knockback.Mul(0.9f);

			if (this.Body != null && !IgnorePos) {
				this.X = this.Body.GetPosition().X;
				this.Y = this.Body.GetPosition().Y - this.Lz;
			} 
		}

		protected bool IgnoreWater() {
			return false;
		}

		protected Void OnTouch(short T, int X, int Y, int Info) {
			if (T == Terrain.WATER && !this.IsFlying()) {
				if (this.HasBuff(Buffs.BURNING) && !(this is BurningMan)) {
					this.RemoveBuff(Buffs.BURNING);

					for (int I = 0; I < 20; I++) {
						SteamFx Fx = new SteamFx();
						Fx.X = this.X + Random.NewFloat(this.W);
						Fx.Y = this.Y + Random.NewFloat(this.H);
						Dungeon.Area.Add(Fx);
					}
				} 
			} else {
				if (!this.IsFlying() && BitHelper.IsBitSet(Info, 0) && !this.HasBuff(Buffs.BURNING)) {
					this.AddBuff(new BurningBuff());
				} 

				if (T == Terrain.LAVA && !this.IsFlying()) {
					if (this is Mob) {
						this.Die();
					} else {
						this.AddBuff(new BurningBuff());
					}

				} else if (!this.IsFlying() && (T == Terrain.HIGH_GRASS || T == Terrain.HIGH_DRY_GRASS)) {
					Dungeon.Level.Set(X, Y, T == Terrain.HIGH_GRASS ? Terrain.GRASS : Terrain.DRY_GRASS);

					for (int I = 0; I < 10; I++) {
						GrassBreakFx Fx = new GrassBreakFx();
						Fx.X = X * 16 + Random.NewFloat(16);
						Fx.Y = Y * 16 + Random.NewFloat(16) - 8;
						Dungeon.Area.Add(Fx);
					}
				} else if (!this.IsFlying() && T == Terrain.VENOM) {
					this.AddBuff(new PoisonedBuff());
				} 
			}

		}

		protected Void Common() {
			float Dt = GetDt();
			this.DoVel();
			this.T += Dt;
			this.Timer += Dt;
			this.Invt = Math.Max(0, this.Invt - Dt);
			this.Invtt = Math.Max(0, this.Invtt - Dt);

			if (!this.Dead) {
				if (this.Velocity.X < 0) {
					this.Flipped = true;
				} else if (this.Velocity.X > 0) {
					this.Flipped = false;
				} 
			} 

			if (this.Velocity.Len2() > 1) {
				this.LastIndex = Dungeon.LongTime;
			} 

			if (this.Body != null) {
				this.Body.SetLinearVelocity(new Vector2(this.Velocity.X + this.Knockback.X, this.Velocity.Y + this.Knockback.Y));
			} 
		}

		protected float GetDt() {
			return Gdx.Graphics.GetDeltaTime();
		}

		protected Void DoVel() {
			float Fr = (IceResitant > 0 && this.Touches[Terrain.ICE] && !this.IsFlying()) ? 1.3f : (this.Touches[Terrain.ICE] && !this.IsFlying() ? 0.2f : (this.SlowLiquidResist == 0 && (this.Touches[Terrain.LAVA]) && !this.IsFlying() ? 0.55f : 1f));
			this.Velocity.X += this.Acceleration.X * Fr;
			this.Velocity.Y += this.Acceleration.Y * Fr;
		}

		public bool IsFlying() {
			return this.Flying;
		}

		public Void SetFlying(bool Flying) {
			this.Flying = Flying;
		}

		public Void ModifySpeed(int Amount) {
			this.Speed += Amount;
			this.MaxSpeed += Amount * 7;
		}

		public bool IsTouching(byte T) {
			return Touches[T];
		}

		public int GetDefense() {
			return this.Defense;
		}

		public float GetSpeed() {
			return Speed;
		}

		public bool IsDead() {
			return this.Dead;
		}

		public Void Die() {
			this.Die(false);
		}

		protected Void Die(bool Force) {
			this.Depth = -1;

			if (this.Dead) {
				return;
			} 

			this.Remove = true;
			this.Dead = true;
		}

		public Void RenderBuffs() {
			foreach (Buff Buff in this.Buffs.Values()) {
				Buff.Render(this);
			}
		}

		public int GetHp() {
			return this.Hp;
		}

		public Void Heal() {
			if (!this.HasFullHealth()) {
				this.ModifyHp(this.GetHpMax() - this.Hp, null);
			} 
		}

		public bool HasFullHealth() {
			return this.Hp == this.HpMax;
		}

		public HpFx ModifyHp(int Amount, Creature From) {
			return this.ModifyHp(Amount, From, false);
		}

		public int GetHpMax() {
			return this.HpMax;
		}

		public HpFx ModifyHp(int Amount, Entity From, bool IgnoreArmor) {
			if (this.IsUnhittable() && Amount < 0) {
				return null;
			} 

			if (this.Done || this.Dead || this.Invtt > 0 || (this.Invt > 0 && !(this is Mob))) {
				return null;
			} else if (this is Mimic && RollBlock()) {
				this.PlaySfx("block");
				this.Invt = InvTime;

				return (HpFx) Dungeon.Area.Add(new HpFx(this, 0, true));
			} 

			bool Hurt = false;

			if (Amount < 0) {
				if (this.Unhittable) {
					return null;
				} 

				if (From is Creature) {
					Amount *= ((Creature) From).RollDamage();
				} else if (From is Projectile) {
					if (((Projectile) From).Owner != null) {
						Amount *= ((Projectile) From).Owner.RollDamage();
					} 
				} 

				if (!IgnoreArmor) {
					Amount += this.Defense;

					if (Amount > 0) {
						Amount = -1;
					} 

					if (From is Creature) {
						((Creature) From).OnHit(this);
					} else if (From is Projectile) {
						if (((Projectile) From).Owner != null) {
							((Projectile) From).Owner.OnHit(this);
						} 
					} 
				} 

				if (this is Player) {
					Amount = Amount < -7 ? -2 : -1;
				} 

				this.Invt = InvTime;
				Hurt = true;
			} 

			if (Player.ShowStats) {
				HpFx Fx = new HpFx(this, Amount, false);
				Dungeon.Area.Add(Fx);
			} 

			if (Hurt) {
				this.DoHurt(Amount);
			} else {
				this.Hp = (int) MathUtils.Clamp(0, this.HpMax, this.Hp + Amount);
			}


			if (Hurt) {
				this.OnHurt(Amount, From);

				if (Settings.Blood) {
					for (int I = 0; I < Random.NewInt(2, 3); I++) {
						BloodSplatFx Fxx = new BloodSplatFx();
						Fxx.SizeMod = 1;
						Fxx.X = X + Random.NewFloat(W) - 8;
						Fxx.Y = Y + Random.NewFloat(H) - 8;
						Dungeon.Area.Add(Fxx);
					}

					if (Random.Chance(50)) {
						for (int I = 0; I < Random.NewInt(1, 3); I++) {
							BloodSplatFx Fxx = new BloodSplatFx();
							Fxx.SizeMod = 1;
							Fxx.Lng = true;
							Fxx.Cntr = true;
							Fxx.A = Random.NewFloat(360);
							Fxx.X = X + Random.NewFloat(W) - 8;
							Fxx.Y = Y + Random.NewFloat(H) - 8;
							Dungeon.Area.Add(Fxx);
						}
					} 

					BloodSplatFx Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.A = From == null ? Random.NewFloat(360) : (float) (Math.ToDegrees(this.GetAngleTo(From.X + From.W, From.Y + From.H) - Math.PI));
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				} 

				BloodFx.Add(this, 10);
			} 

			this.CheckDeath();

			return null;
		}

		public bool IsUnhittable() {
			return Unhittable;
		}

		public bool RollBlock() {
			return false;
		}

		public float RollDamage() {
			return this.Touches[Terrain.ICE] ? 2 : 1;
		}

		public Void OnHit(Creature Who) {

		}

		protected Void DoHurt(int A) {
			this.Hp = Math.Max(0, this.Hp + A);
		}

		protected Void OnHurt(int A, Entity From) {
			Graphics.Delay(40);
		}

		protected Void CheckDeath() {
			if (this.Hp == 0) {
				this.ShouldDie = true;
			} 
		}

		public Void SetUnhittable(bool Unhittable) {
			this.Unhittable = Unhittable;
		}

		public Void SetHpMax(int HpMax) {
			this.HpMax = Math.Max(2, HpMax);

			if (this is Player) {
				Player Self = (Player) this;
				this.HpMax = Math.Min(32 - Self.GetGoldenHearts() - Self.GetIronHearts(), this.HpMax);
			} 

			this.Hp = (int) MathUtils.Clamp(0, this.HpMax, this.Hp);
		}

		public Void ModifyHpMax(int Amount) {
			this.SetHpMax(this.HpMax + Amount);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) this.Hp);
			Writer.WriteInt16((short) this.HpMax);
			Writer.WriteInt32(this.Buffs.Size());

			foreach (Buff Buff in this.Buffs.Values()) {
				Writer.WriteString(Buff.GetClass().GetName());
				Writer.WriteFloat(Buff.GetDuration());
			}
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Hp = Reader.ReadInt16();
			this.HpMax = Reader.ReadInt16();
			int Count = Reader.ReadInt32();

			for (int I = 0; I < Count; I++) {
				string T = Reader.ReadString();
				Class Clazz;

				try {
					Clazz = Class.ForName(T);
					Constructor Constructor = Clazz.GetConstructor();
					Object Object = Constructor.NewInstance();
					Buff Buff = (Buff) Object;
					Buff.SetOwner(this);
					Buff.SetDuration(Reader.ReadFloat());
					this.AddBuff(Buff);
				} catch (Exception) {
					E.PrintStackTrace();
				}
			}

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0);
				this.Lz = this.Z;
			} 
		}

		public Void AddBuff(Buff Buff) {
			if (Buff is BurningBuff && Dungeon.Level is IceLevel) {
				return;
			} 

			if (this.CanHaveBuff(Buff)) {
				Buff B = this.Buffs.Get(Buff.GetId());

				if (B != null) {

				} else {
					this.Buffs.Put(Buff.GetId(), Buff);
					Buff.SetOwner(this);
					Buff.OnStart();
				}


				if (Buff is PoisonedBuff) {
					this.RemoveBuff(Buffs.BURNING);
				} 
			} 
		}

		protected bool CanHaveBuff(Buff Buff) {
			if (this.Unhittable) {
				if (Buff is BurningBuff || Buff is PoisonedBuff) {
					return false;
				} 
			} 

			return true;
		}

		public Void RemoveBuff(string Buff) {
			Buff Instance = this.Buffs.Get(Buff);

			if (Instance != null) {
				Instance.OnEnd();
				this.Buffs.Remove(Buff);
				this.OnBuffRemove(Instance);
			} 
		}

		public Void OnBuffRemove(Buff Buff) {

		}

		public Body GetBody() {
			return this.Body;
		}

		public Void KnockBackFrom(Entity From, float Force) {
			if (From == null || this.Unhittable) {
				return;
			} 

			Force *= 100;
			float A = From.GetAngleTo(this.X + this.W / 2, this.Y + this.H / 2);
			this.Knockback.X += Math.Cos(A) * Force * KnockbackMod;
			this.Knockback.Y += Math.Sin(A) * Force * KnockbackMod;
		}

		public bool IsFlipped() {
			return this.Flipped;
		}

		public Room GetRoom() {
			return Room;
		}
	}
}
