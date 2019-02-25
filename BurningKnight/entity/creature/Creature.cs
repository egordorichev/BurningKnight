using BurningKnight.entity.creature.buff;
using BurningKnight.entity.creature.fx;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.common;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.creature.mob.tech;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.entity.level.levels.ice;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.save;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.file;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity {
		public float A = 1f;
		protected Point Acceleration = new Point();
		protected Body Body;
		protected Dictionary<string, Buff> Buffs = new Dictionary<>();
		protected int Damage = 2;
		protected bool Dead;
		protected int Defense;
		public bool ExplosionBlock;
		protected bool Flipped;
		protected bool Flying;
		public bool Freezed;
		public int Hh;
		protected int Hp;
		protected int HpMax;
		public int Hw;
		public int Hx;
		public int Hy;
		public int IceResitant;
		protected bool IgnoreAcceleration;
		protected bool IgnorePos;
		public bool Invisible;
		protected float Invt;
		public float InvTime = 0.2f;
		protected float Invtt;
		public Point Knockback = new Point();
		public float KnockbackMod = 1f;
		public long LastIndex;
		public float Lz;
		public float MaxSpeed = 90f;
		protected float Mul = 0.7f;
		public bool Penetrates;
		public bool Poisoned;
		public bool Remove;
		public Room Room;
		protected bool ShouldDie;
		public int SlowLiquidResist = 0;
		public float Speed = 10f;
		protected float Timer;
		protected bool[] Touches = new bool[Terrain.SIZE];
		protected bool Unhittable;
		public float Z;

		public float GetWeaponAngle() {
			var Aim = GetAim();

			return GetAngleTo(Aim.X, Aim.Y);
		}

		public Point GetAim() {
			return Input.Instance.WorldMouse;
		}

		public Body CreateSimpleBody(int X, int Y, int W, int H, BodyDef.BodyType Type, bool Sensor) {
			Hx = X;
			Hy = Y;
			Hw = W;
			Hh = H;
			Body Body = World.CreateSimpleBody(this, X, Y, W, H, Type, Sensor);

			if (Body != null) Body.SetSleepingAllowed(false);

			return Body;
		}

		public void ModifyDefense(int Amount) {
			Defense += Amount;
		}

		public void Tp(float X, float Y) {
			this.X = X;
			this.Y = Y;

			if (Body != null) {
				World.CheckLocked(Body).SetTransform(this.X, this.Y + Z, 0);
				Lz = Z;
			}
		}

		public float GetInvt() {
			return Invt;
		}

		public void SetInvt(float Invt) {
			Invtt = Invt;
		}

		public override void Init() {
			base.Init();
			T = Random.NewFloat(1024);
			Hp = HpMax;
		}

		public override void Destroy() {
			base.Destroy();

			if (Body != null) Body = World.RemoveBody(Body);
		}

		public override void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y, W, H, 6);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (IsFlying() && Entity is Level) return false;

			if (Entity is Creature) {
				if (HasBuff(Buffs.BURNING)) ((Creature) Entity).AddBuff(new BurningBuff());

				if (!(this is Player && Entity is Mummy)) return false;
			}
			else if (Entity is ItemHolder) {
				return false;
			}
			else if (Entity is Weapon && ((Weapon) Entity).GetOwner() == this) {
				return false;
			}

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public bool HasBuff(string Buff) {
			return Buffs.ContainsKey(Buff);
		}

		protected void OnRoomChange() {
		}

		public void DeathEffect(AnimationData Killed) {
			LevelSave.Remove(this);

			if (Settings.Gore)
				foreach (Animation.Frame Frame in Killed.GetFrames()) {
					var Fx = new GoreFx();
					Fx.Texture = Frame.Frame;
					Fx.X = this.X + W / 2;
					Fx.Y = this.Y + H / 2;
					Dungeon.Area.Add(Fx);
				}

			if (Settings.Blood) {
				for (var I = 0; I < Random.NewInt(2, 3); I++) {
					var Fxx = new BloodSplatFx();
					Fxx.SizeMod = 2;
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}

				for (var I = 0; I < Random.NewInt(2, 5); I++) {
					var Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.A = Random.NewFloat(360);
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}

				for (var I = 0; I < Random.NewInt(3, 5); I++) {
					var Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.Cntr = true;
					Fxx.A = Random.NewFloat(360);
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}
			}

			Poof();
			BloodFx.Add(this, 20);
		}

		public void Poof() {
			for (var I = 0; I < 10; I++) {
				var Fx = new PoofFx();
				Fx.X = this.X + W / 2;
				Fx.Y = this.Y + H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override void Update(float Dt) {
			Arrays.Fill(Touches, false);

			for (var X = (int) Math.Floor((Hx + this.X) / 16); X < Math.Ceil((Hx + this.X + Hw) / 16); X++)
			for (var Y = (int) Math.Floor((Hy + this.Y + 8) / 16); Y < Math.Ceil((Hy + this.Y + 8 + Hh / 3) / 16); Y++) {
				if (X < 0 || Y < 0 || X >= Level.GetWidth() || Y >= Level.GetHeight()) continue;

				if (CollisionHelper.Check(Hx + this.X, Hy + this.Y, Hw, Hh / 3, X * 16, Y * 16 - 8, 16, 16)) {
					int I = Level.ToIndex(X, Y);
					int Info = Dungeon.Level.GetInfo(I);
					byte T = Dungeon.Level.Get(I);
					OnTouch(T, X, Y, Info);
					Touches[T] = true;
					byte L = Dungeon.Level.LiquidData[I];

					if (L > 0) {
						Rectangle R = Level.COLLISIONS[Dungeon.Level.LiquidVariants[I]];

						if (CollisionHelper.Check(Hx + this.X, Hy + this.Y, Hw, Hh / 3, X * 16 + R.X, Y * 16 - 8 + R.Y, R.Width, R.Height)) {
							Touches[L] = true;
							OnTouch(L, X, Y, Info);
						}
					}
				}
			}

			if (!IgnoreAcceleration) {
				Acceleration.X = 0;
				Acceleration.Y = 0;
			}

			Buff[] Buffs = this.Buffs.Values().ToArray({
			});
			var Sx = (int) (this.X + W / 2);
			var Sy = (int) (this.Y + H / 2);

			for (var I = Buffs.Length - 1; I >= 0; I--) {
				if (Buffs[I] is BurningBuff) {
					InGameState.Burning = true;

					if (!(this is BurningMan) && !IsFlying()) Dungeon.Level.SetOnFire(Level.ToIndex(Sx / 16, Sy / 16), true, false);
				}

				Buffs[I].Update(Dt);
			}

			Room Room = Dungeon.Level.FindRoomFor(Sx, Sy);

			if (Room != this.Room) {
				this.Room = Room;
				OnRoomChange();
			}

			base.Update(Dt);

			if (ShouldDie) {
				Die();
				ShouldDie = false;
			}

			if (Hp == 0 && !Dead) Die(true);

			if (Remove) {
				Destroy();
				Remove = false;
			}

			if (Dead) return;

			if (Freezed) {
				Invt = Math.Max(0, Invt - Dt);
				Invtt = Math.Max(0, Invtt - Dt);
				Velocity.X = 0;
				Velocity.Y = 0;
				Body.SetLinearVelocity(new Vector2(Velocity.X + Knockback.X, Velocity.Y + Knockback.Y));

				return;
			}

			if (!IsFlying() && Touches[Terrain.WATER] && !IgnoreWater() && !(this is Tank) && (!(this is Player) || !((Player) this).IsRolling())) Velocity.Y -= Dt * 300;

			if (this is Player && ((Player) this).IsRolling())
				Velocity.Mul(Mul);
			else
				Velocity.Mul(Touches[Terrain.COBWEB] && !IsFlying() ? 0.5f : (IceResitant == 0 && Touches[Terrain.ICE] && !IsFlying() ? 0.95f : Mul));


			Knockback.Mul(0.9f);

			if (Body != null && !IgnorePos) {
				this.X = Body.GetPosition().X;
				this.Y = Body.GetPosition().Y - Lz;
			}
		}

		protected bool IgnoreWater() {
			return false;
		}

		protected void OnTouch(short T, int X, int Y, int Info) {
			if (T == Terrain.WATER && !IsFlying()) {
				if (HasBuff(Buffs.BURNING) && !(this is BurningMan)) {
					RemoveBuff(Buffs.BURNING);

					for (var I = 0; I < 20; I++) {
						var Fx = new SteamFx();
						Fx.X = this.X + Random.NewFloat(W);
						Fx.Y = this.Y + Random.NewFloat(H);
						Dungeon.Area.Add(Fx);
					}
				}
			}
			else {
				if (!IsFlying() && BitHelper.IsBitSet(Info, 0) && !HasBuff(Buffs.BURNING)) AddBuff(new BurningBuff());

				if (T == Terrain.LAVA && !IsFlying()) {
					if (this is Mob)
						Die();
					else
						AddBuff(new BurningBuff());
				}
				else if (!IsFlying() && (T == Terrain.HIGH_GRASS || T == Terrain.HIGH_DRY_GRASS)) {
					Dungeon.Level.Set(X, Y, T == Terrain.HIGH_GRASS ? Terrain.GRASS : Terrain.DRY_GRASS);

					for (var I = 0; I < 10; I++) {
						var Fx = new GrassBreakFx();
						Fx.X = X * 16 + Random.NewFloat(16);
						Fx.Y = Y * 16 + Random.NewFloat(16) - 8;
						Dungeon.Area.Add(Fx);
					}
				}
				else if (!IsFlying() && T == Terrain.VENOM) {
					AddBuff(new PoisonedBuff());
				}
			}
		}

		protected void Common() {
			var Dt = GetDt();
			DoVel();
			T += Dt;
			Timer += Dt;
			Invt = Math.Max(0, Invt - Dt);
			Invtt = Math.Max(0, Invtt - Dt);

			if (!Dead) {
				if (Velocity.X < 0)
					Flipped = true;
				else if (Velocity.X > 0) Flipped = false;
			}

			if (Velocity.Len2() > 1) LastIndex = Dungeon.LongTime;

			if (Body != null) Body.SetLinearVelocity(new Vector2(Velocity.X + Knockback.X, Velocity.Y + Knockback.Y));
		}

		protected float GetDt() {
			return Gdx.Graphics.GetDeltaTime();
		}

		protected void DoVel() {
			var Fr = IceResitant > 0 && Touches[Terrain.ICE] && !IsFlying() ? 1.3f : (Touches[Terrain.ICE] && !IsFlying() ? 0.2f : (SlowLiquidResist == 0 && Touches[Terrain.LAVA] && !IsFlying() ? 0.55f : 1f));
			Velocity.X += Acceleration.X * Fr;
			Velocity.Y += Acceleration.Y * Fr;
		}

		public bool IsFlying() {
			return Flying;
		}

		public void SetFlying(bool Flying) {
			this.Flying = Flying;
		}

		public void ModifySpeed(int Amount) {
			Speed += Amount;
			MaxSpeed += Amount * 7;
		}

		public bool IsTouching(byte T) {
			return Touches[T];
		}

		public int GetDefense() {
			return Defense;
		}

		public float GetSpeed() {
			return Speed;
		}

		public bool IsDead() {
			return Dead;
		}

		public void Die() {
			Die(false);
		}

		protected void Die(bool Force) {
			Depth = -1;

			if (Dead) return;

			Remove = true;
			Dead = true;
		}

		public void RenderBuffs() {
			foreach (Buff Buff in Buffs.Values()) Buff.Render(this);
		}

		public int GetHp() {
			return Hp;
		}

		public void Heal() {
			if (!HasFullHealth()) ModifyHp(GetHpMax() - Hp, null);
		}

		public bool HasFullHealth() {
			return Hp == HpMax;
		}

		public HpFx ModifyHp(int Amount, Creature From) {
			return ModifyHp(Amount, From, false);
		}

		public int GetHpMax() {
			return HpMax;
		}

		public HpFx ModifyHp(int Amount, Entity From, bool IgnoreArmor) {
			if (IsUnhittable() && Amount < 0) return null;

			if (Done || Dead || Invtt > 0 || Invt > 0 && !(this is Mob)) {
				return null;
			}

			if (this is Mimic && RollBlock()) {
				PlaySfx("block");
				Invt = InvTime;

				return (HpFx) Dungeon.Area.Add(new HpFx(this, 0, true));
			}

			var Hurt = false;

			if (Amount < 0) {
				if (Unhittable) return null;

				if (From is Creature)
					Amount *= ((Creature) From).RollDamage();
				else if (From is Projectile)
					if (((Projectile) From).Owner != null)
						Amount *= ((Projectile) From).Owner.RollDamage();

				if (!IgnoreArmor) {
					Amount += Defense;

					if (Amount > 0) Amount = -1;

					if (From is Creature)
						((Creature) From).OnHit(this);
					else if (From is Projectile)
						if (((Projectile) From).Owner != null)
							((Projectile) From).Owner.OnHit(this);
				}

				if (this is Player) Amount = Amount < -7 ? -2 : -1;

				Invt = InvTime;
				Hurt = true;
			}

			if (Player.ShowStats) {
				var Fx = new HpFx(this, Amount, false);
				Dungeon.Area.Add(Fx);
			}

			if (Hurt)
				DoHurt(Amount);
			else
				Hp = (int) MathUtils.Clamp(0, HpMax, Hp + Amount);


			if (Hurt) {
				OnHurt(Amount, From);

				if (Settings.Blood) {
					for (var I = 0; I < Random.NewInt(2, 3); I++) {
						var Fxx = new BloodSplatFx();
						Fxx.SizeMod = 1;
						Fxx.X = X + Random.NewFloat(W) - 8;
						Fxx.Y = Y + Random.NewFloat(H) - 8;
						Dungeon.Area.Add(Fxx);
					}

					if (Random.Chance(50))
						for (var I = 0; I < Random.NewInt(1, 3); I++) {
							var Fxx = new BloodSplatFx();
							Fxx.SizeMod = 1;
							Fxx.Lng = true;
							Fxx.Cntr = true;
							Fxx.A = Random.NewFloat(360);
							Fxx.X = X + Random.NewFloat(W) - 8;
							Fxx.Y = Y + Random.NewFloat(H) - 8;
							Dungeon.Area.Add(Fxx);
						}

					var Fxx = new BloodSplatFx();
					Fxx.SizeMod = 1;
					Fxx.Lng = true;
					Fxx.A = From == null ? Random.NewFloat(360) : (float) Math.ToDegrees(GetAngleTo(From.X + From.W, From.Y + From.H) - Math.PI);
					Fxx.X = X + Random.NewFloat(W) - 8;
					Fxx.Y = Y + Random.NewFloat(H) - 8;
					Dungeon.Area.Add(Fxx);
				}

				BloodFx.Add(this, 10);
			}

			CheckDeath();

			return null;
		}

		public bool IsUnhittable() {
			return Unhittable;
		}

		public bool RollBlock() {
			return false;
		}

		public float RollDamage() {
			return Touches[Terrain.ICE] ? 2 : 1;
		}

		public void OnHit(Creature Who) {
		}

		protected void DoHurt(int A) {
			Hp = Math.Max(0, Hp + A);
		}

		protected void OnHurt(int A, Entity From) {
			Graphics.Delay(40);
		}

		protected void CheckDeath() {
			if (Hp == 0) ShouldDie = true;
		}

		public void SetUnhittable(bool Unhittable) {
			this.Unhittable = Unhittable;
		}

		public void SetHpMax(int HpMax) {
			this.HpMax = Math.Max(2, HpMax);

			if (this is Player) {
				var Self = (Player) this;
				this.HpMax = Math.Min(32 - Self.GetGoldenHearts() - Self.GetIronHearts(), this.HpMax);
			}

			Hp = (int) MathUtils.Clamp(0, this.HpMax, Hp);
		}

		public void ModifyHpMax(int Amount) {
			SetHpMax(HpMax + Amount);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) Hp);
			Writer.WriteInt16((short) HpMax);
			Writer.WriteInt32(Buffs.Size());

			foreach (Buff Buff in Buffs.Values()) {
				Writer.WriteString(Buff.GetClass().GetName());
				Writer.WriteFloat(Buff.GetDuration());
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Hp = Reader.ReadInt16();
			HpMax = Reader.ReadInt16();
			var Count = Reader.ReadInt32();

			for (var I = 0; I < Count; I++) {
				var T = Reader.ReadString();
				Class Clazz;

				try {
					Clazz = Class.ForName(T);
					Constructor Constructor = Clazz.GetConstructor();
					Object Object = Constructor.NewInstance();
					var Buff = (Buff) Object;
					Buff.SetOwner(this);
					Buff.SetDuration(Reader.ReadFloat());
					AddBuff(Buff);
				}
				catch (Exception) {
					E.PrintStackTrace();
				}
			}

			if (Body != null) {
				World.CheckLocked(Body).SetTransform(this.X, this.Y + Z, 0);
				Lz = Z;
			}
		}

		public void AddBuff(Buff Buff) {
			if (Buff is BurningBuff && Dungeon.Level is IceLevel) return;

			if (CanHaveBuff(Buff)) {
				Buff B = Buffs.Get(Buff.GetId());

				if (B != null) {
				}
				else {
					Buffs.Put(Buff.GetId(), Buff);
					Buff.SetOwner(this);
					Buff.OnStart();
				}


				if (Buff is PoisonedBuff) RemoveBuff(Buffs.BURNING);
			}
		}

		protected bool CanHaveBuff(Buff Buff) {
			if (Unhittable)
				if (Buff is BurningBuff || Buff is PoisonedBuff)
					return false;

			return true;
		}

		public void RemoveBuff(string Buff) {
			Buff Instance = Buffs.Get(Buff);

			if (Instance != null) {
				Instance.OnEnd();
				Buffs.Remove(Buff);
				OnBuffRemove(Instance);
			}
		}

		public void OnBuffRemove(Buff Buff) {
		}

		public Body GetBody() {
			return Body;
		}

		public void KnockBackFrom(Entity From, float Force) {
			if (From == null || Unhittable) return;

			Force *= 100;
			var A = From.GetAngleTo(this.X + W / 2, this.Y + H / 2);
			Knockback.X += Math.Cos(A) * Force * KnockbackMod;
			Knockback.Y += Math.Sin(A) * Force * KnockbackMod;
		}

		public bool IsFlipped() {
			return Flipped;
		}

		public Room GetRoom() {
			return Room;
		}
	}
}