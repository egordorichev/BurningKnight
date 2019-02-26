using BurningKnight.entity.creature.mob.prefix;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.entity.level.rooms;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob {
	public class Mob : Creature {
		public static int MaxId;
		public static bool Challenge;
		public static float SpeedMod = 1f;
		public static float ShotSpeedMod = 1f;
		public static List<Mob> All = new List<>();
		public static List<Mob> Every = new List<>();
		private static TextureRegion HideSign = Graphics.GetTexture("ui-hide");
		private static TextureRegion NoticeSign = Graphics.GetTexture("ui-notice");
		public static ShaderProgram Shader;
		public static ShaderProgram Frozen;
		protected State Ai;

		private RayCastCallback Callback = new RayCastCallback {

			public override float ReportRayFixture(Fixture Fixture,
			Vector2 Point,
			Vector2 Normal,
			float Fraction) {
			Object Data = Fixture.GetBody().GetUserData();
			if (!Fixture.IsSensor() && Data is Entity && !(Data is Level || Data is ItemHolder || (Data is Door && ((Door) Data).IsOpen()))) {
			if (Fraction < ClosestFraction) {
			ClosestFraction = Fraction;
			Last = (Entity) Data;
		}

		private float ClosestFraction = 1.0f;
		public List<Player> Colliding = new List<>();
		protected bool Drop;
		private float Fa;
		public bool FlippedVert;
		public float HideSignT;
		private int Id;
		protected bool IgnoreRooms;
		private Entity Last;
		public Point LastSeen;
		public bool Nodebuffs;
		public bool NoLoot;
		public float NoticeSignT;
		protected Prefix Prefix;
		public bool Stupid = false;
		protected float Sx = 1;
		protected float Sy = 1;
		public Creature Target;
		private bool WasFreezed;
		private bool WasPoisoned;

		static Mob() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/outline.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());


			Frozen = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/ice.frag").ReadString());

			if (!Frozen.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Frozen.GetLog());
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public class GetOutState : State {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Self.IsLow() ? 100000000f : Random.NewFloat(3f, 6f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				MoveFrom(Self.LastSeen, 25f, 5f);

				if (T >= Delay) Self.Become(GetState());
			}

			protected string GetState() {
				return "idle";
			}
		}

		public class State<T> where T : Mob {
			public Point NextPathPoint;
			public T Self;
			public float T;
			public Room Target;
			public Point TargetPoint;

			public void Update(float Dt) {
				T += Dt;
			}

			public void OnEnter() {
			}

			public void OnExit() {
			}

			public bool FlyTo(Point Point, float S, float Ds) {
				var Dx = Point.X - Self.X - Self.W / 2;
				var Dy = Point.Y - Self.Y - Self.H / 2;
				var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

				if (D < Ds) return true;

				Self.Acceleration.X += Dx / D * S;
				Self.Acceleration.Y += Dy / D * S;

				return false;
			}

			public bool MoveRightTo(Point Point, float S, float D) {
				float Ds = Self.MoveToPoint(Point.X + 8, Point.Y + 8, S);
				var Dd = Self.GetDistanceTo(Point.X + 8, Point.Y + 8);

				if (Ds < 4f || Dd < D) return Dd <= D;

				return false;
			}

			public bool MoveTo(Point Point, float S, float D) {
				if (NextPathPoint == null) {
					NextPathPoint = Self.GetCloser(Point);

					if (NextPathPoint == null) return false;
				}

				float Ds = Self.MoveToPoint(NextPathPoint.X + 8, NextPathPoint.Y, S);
				var Dd = Self.GetDistanceTo(Point.X + 8, Point.Y + 8);

				if (Ds < 4f || Dd < D) {
					NextPathPoint = null;

					return Dd <= D;
				}

				return false;
			}

			public bool MoveFrom(Point Point, float S, float D) {
				if (TargetPoint == null) {
					if (Self.Target == null) {
						Self.Target = Player.Instance;

						if (!Self.Saw) {
							Self.Saw = true;
							Self.NoticeSignT = 2f;
							Self.PlaySfx("enemy_alert");
						}
					}

					Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
					TargetPoint = Self.GetFar(Self.LastSeen);

					if (TargetPoint == null) {
						Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);

						return false;
					}
				}

				if (NextPathPoint == null) {
					NextPathPoint = Self.GetCloser(TargetPoint);

					if (NextPathPoint == null) {
						TargetPoint = null;

						return false;
					}
				}

				float Ds = Self.MoveToPoint(NextPathPoint.X + 8, NextPathPoint.Y, S);
				var Dd = Self.GetDistanceTo(TargetPoint.X + 8, TargetPoint.Y + 8);

				if (Ds < 4f || Dd < D) {
					NextPathPoint = null;

					return Dd <= D;
				}

				return false;
			}

			public void CheckForPlayer() {
				CheckForPlayer(false);
			}

			public void CheckForPlayer(bool Force) {
				if (Self.Target != null) {
					if (Self.Target.Room != Self.Room) {
						Self.Saw = false;
						Self.LastSeen = null;

						return;
					}

					Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
				}

				if (Target == null && Force) Self.FindTarget(true);

				if (Self.Target != null) {
					if (Self.Target.Room == Self.Room && !Self.Saw && Self.CanSee(Self.Target)) {
						Self.Saw = true;
						Self.PlaySfx("enemy_alert");
						Self.NoticeSignT = 2f;

						if (!Self.State.Equals("chase") && !Self.State.Equals("runaway")) Self.Become("alerted");
					}
				}
				else if (Self.Saw) {
					Self.Saw = false;
				}
			}

			public void FindNearbyPoint() {
				if (TargetPoint != null) return;

				if (Self.Room != null) {
					if (true)
						Target = Self.Room;
					else
						for (var I = 0; I < 10; I++) {
							Target = Self.Room.Neighbours.Get(Random.NewInt(Self.Room.Neighbours.Size()));

							if (Target != Self.LastRoom && Target != Self.Room) {
								Self.LastRoom = Self.Room;

								break;
							}

							if (I == 9) Self.LastRoom = Self.Room;
						}


					var Found = false;

					for (var I = 0; I < Target.GetWidth() * Target.GetHeight(); I++) {
						TargetPoint = Target.GetRandomCell();

						if (Dungeon.Level.IsValid((int) TargetPoint.X, (int) TargetPoint.Y) &&
						    (Self.ToWater ? Dungeon.Level.Get((int) TargetPoint.X, (int) TargetPoint.Y) == Terrain.WATER : Dungeon.Level.CheckFor((int) TargetPoint.X, (int) TargetPoint.Y, Terrain.PASSABLE))) {
							Found = true;
							TargetPoint.Mul(16);

							break;
						}
					}

					if (!Found) {
						Target = null;
						TargetPoint = null;
					}
					else {
						Self.ToWater = false;
					}
				}
			}
		}
	}

	return ClosestFraction;
}

};
private float LastBlood;
private float LastSplat;
public bool Friendly = false;
protected bool IgnoreVel;
public bool Dd;
public bool NoDrop;
public Room LastRoom;
public bool ToWater;
public bool Saw;
protected override bool CanHaveBuff(Buff Buff) {
if (Nodebuffs && (Buff is FrozenBuff || Buff is BurningBuff || Buff is PoisonedBuff)) {
return false;
}
return base.CanHaveBuff(Buff);
}
public override void Init() {
base.Init();
Id = MaxId;
MaxId++;
if (!(this is BurningKnight) && !(this is Npc) && !(this is Mimic) && !(this is IceElemental)) {
All.Add(this);
}
Every.Add(this);
if (Random.Chance(50)) {
this.Become("roam");
}
}
public Mob Generate() {
if (Challenge) {
this.GeneratePrefix();
}
this.Hp = (int) (this.HpMax * 0.75f);
return this;
}
public void RenderStats() {
if (Player.ShowStats) {
Graphics.Print(this.Hp + "/" + this.HpMax, Graphics.Small, this.X, this.Y + this.Z);
}
}
public void RenderSigns() {
TextureRegion Region = (this.NoticeSignT > 0 ? NoticeSign : null);
if (Region != null) {
float T = Math.Max(this.HideSignT, this.NoticeSignT);
if (T <= 0.2f) {
Graphics.Batch.SetColor(1, 1, 1, T * 5);
} else if (T >= 1.8f) {
Graphics.Batch.SetColor(1, 1, 1, (T - 1.8f) * 5);
}
Graphics.Render(Region, this.X + (this.W - Region.GetRegionWidth()) / 2 + Region.GetRegionWidth() / 2, (float) (this.Y + this.H + 2 + Math.Cos(T * 5) * 5.5f + Region.GetRegionHeight() / 2), (float) (Math.Cos(T * 4) * 30f), Region.
GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false);
Graphics.Batch.SetColor(1, 1, 1, 1);
}
}
public void GeneratePrefix() {
if (this.Prefix == null && Dungeon.Depth != -3) {
this.Prefix = PrefixPool.Instance.Generate();
this.Prefix.Apply(this);
this.Prefix.OnGenerate(this);
}
}
public float GetOx() {
return W / 2;
}
public float GetOy() {
return 0;
}
public void RenderWithOutline(AnimationData Data) {
TextureRegion Region = Data.GetCurrent().Frame;
float W = Region.GetRegionWidth();
if (this.Prefix != null) {
Color Color = this.Prefix.GetColor();
Graphics.Batch.End();
Shader.Begin();
Shader.SetUniformf("u_a", 1f);
Shader.SetUniformf("u_color", new Vector3(Color.R, Color.G, Color.B));
Shader.End();
Graphics.Batch.SetShader(Shader);
Graphics.Batch.Begin();
for (int Xx = -1; Xx < 2; Xx++) {
for (int Yy = -1; Yy < 2; Yy++) {
if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
Graphics.Render(Region, X + Xx + W / 2, Y + Z + Yy + GetOy(), 0, GetOx(), GetOy(), false, false, Sx * (Flipped ? -1f : 1f), Sy * (FlippedVert ? -1 : 1));
}
}
}
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
Graphics.Batch.Begin();
}
if (this.Fa > 0) {
Graphics.Batch.End();
Frozen.Begin();
Frozen.SetUniformf("time", Dungeon.Time);
Frozen.SetUniformf("f", this.Fa);
Frozen.SetUniformf("a", this.A);
Frozen.SetUniformf("freezed", this.WasFreezed ? 1f : 0f);
Frozen.SetUniformf("poisoned", this.WasPoisoned ? 1f : 0f);
Frozen.End();
Graphics.Batch.SetShader(Frozen);
Graphics.Batch.Begin();
}
Graphics.Batch.SetColor(1, 1, 1, this.A);
Graphics.Render(Region, Math.Round(X + W / 2), Math.Round(Y + Z) + GetOy(), 0, GetOx(), GetOy(), false, false, Sx * (Flipped ? -1 : 1), Sy * (FlippedVert ? -1 : 1));
if (this.Freezed || this.Poisoned) {
this.Fa += (1 - this.Fa) * Gdx.Graphics.GetDeltaTime() * 3f;
this.WasFreezed = this.Freezed;
this.WasPoisoned = this.Poisoned;
} else {
this.Fa += (0 - this.Fa) * Gdx.Graphics.GetDeltaTime() * 3f;
if (this.Fa <= 0) {
this.WasFreezed = false;
this.WasPoisoned = false;
}
}
if (this.Fa > 0) {
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
Graphics.Batch.Begin();
}
}
public override void Load(FileReader Reader) {
base.Load(Reader);
int Id = Reader.ReadInt16();
if (Id > 0) {
this.Prefix = PrefixPool.Instance.GetModifier(Id - 1);
this.Prefix.Apply(this);
}
NoLoot = Reader.ReadBoolean();
}
public override void Save(FileWriter Writer) {
base.Save(Writer);
if (this.Prefix == null) {
Writer.WriteInt16((short) 0);
} else {
Writer.WriteInt16((short) (this.Prefix.Id + 1));
}
Writer.WriteBoolean(NoLoot);
}
public bool CanSee(Creature Player) {
return CanSee(Player, 0);
}
public bool CanSee(Creature Player, int Ignore) {
if (Player == null) {
return false;
}
if (!(this is Shopkeeper || this is BurningKnight)) {
if (Player.Room != this.Room) {
return false;
}
}
float X = this.X + this.W / 2;
float Y = this.Y + this.H / 2;
float X2 = Player.X + Player.W / 2;
float Y2 = Player.Y + Player.H / 2;
return Dungeon.Level.CanSee((int) Math.Floor(X / 16), (int) Math.Floor(Y / 16), (int) Math.Floor(X2 / 16), (int) Math.Floor(Y2 / 16), Ignore) == 0;
}
public bool CanSeePoint(Point Player) {
if (Player == null) {
return false;
}
float X = this.X + this.W / 2;
float Y = this.Y + this.H / 2;
float X2 = Player.X;
float Y2 = Player.Y;
return Dungeon.Level.CanSee((int) Math.Floor(X / 16), (int) Math.Floor(Y / 16), (int) Math.Floor(X2 / 16), (int) Math.Floor(Y2 / 16), 0) == 0;
}
public Point GetCloser(Point Target) {
int From = (int) (Math.Floor((this.X + this.W / 2) / 16) + Math.Floor((this.Y + 12) / 16) * Level.GetWidth());
int To = (int) (Math.Floor((Target.X + this.W / 2) / 16) + Math.Floor((Target.Y + 12) / 16) * Level.GetWidth());
if (!Dungeon.Level.CheckFor(To, Terrain.PASSABLE)) {
To -= Level.GetWidth();
}
if (!Dungeon.Level.CheckFor(From, Terrain.PASSABLE)) {
From -= Level.GetWidth();
}
int Step = PathFinder.GetStep(From, To, Dungeon.Level.GetPassable());
if (Step != -1) {
Point P = new Point();
P.X = Step % Level.GetWidth() * 16;
P.Y = (float) (Math.Floor(Step / Level.GetWidth()) * 16);
return P;
}
return null;
}
public Point GetFar(Point Target) {
if (Target == null) {
return null;
}
int Pos = (int) (Math.Floor((Target.X + this.W / 2) / 16) + Math.Floor((Target.Y + 12) / 16) * Level.GetWidth());
int From = (int) (Math.Floor((this.X + this.W / 2) / 16) + Math.Floor((this.Y + 12) / 16) * Level.GetWidth());
if (!Dungeon.Level.CheckFor(Pos, Terrain.PASSABLE)) {
Pos -= Level.GetWidth();
}
if (!Dungeon.Level.CheckFor(From, Terrain.PASSABLE)) {
From -= Level.GetWidth();
}
PathFinder.GetStepBack(From, Pos, Dungeon.Level.GetPassable(), Pos);
if (PathFinder.LastStep != -1) {
Point P = new Point();
P.X = PathFinder.LastStep % Level.GetWidth() * 16;
P.Y = (float) (Math.Floor(PathFinder.LastStep / Level.GetWidth()) * 16);
return P;
}
return null;
}
public float GetWeight() {
return 1;
}
public bool IsLow() {
return this.Hp != this.HpMax && this.Hp <= Math.Ceil(((float) this.HpMax) / 4) && !(this.State.Equals("unactive"));
}
public override void Update(float Dt) {
base.Update(Dt * SpeedMod);
this.NoticeSignT = Math.Max(0, this.NoticeSignT - Dt);
this.HideSignT = Math.Max(0, this.HideSignT - Dt);
if (this.IsLow()) {
this.LastSplat += Dt;
this.LastBlood += Dt;
if (this.LastBlood > 0.1f) {
this.LastBlood = 0;
BloodDropFx Fx = new BloodDropFx();
Fx.Owner = this;
Dungeon.Area.Add(Fx);
}
if (this.LastSplat >= 1f && Settings.Blood) {
this.LastSplat = 0;
BloodSplatFx Fxx = new BloodSplatFx();
Fxx.X = X + Random.NewFloat(W) - 8;
Fxx.Y = Y + Random.NewFloat(H) - 8;
Dungeon.Area.Add(Fxx);
}
}
if (this.Drop) {
if (Dungeon.Depth != -3) {
if (this.Prefix != null) {
this.Prefix.OnDeath(this);
}
this.Drop = false;
if (!NoDrop && !NoLoot) {
List<Item> Items = this.GetDrops();
foreach (Item Item in Items) {
if (Chest.IsVeganFine(Item.GetClass())) {
ItemHolder Holder = new ItemHolder(Item);
if (this is Boss) {
Point Point = Room.GetCenter();
Holder.X = Point.X * 16;
Holder.Y = Point.Y * 16;
} else {
Holder.X = this.X;
Holder.Y = this.Y;
}
Holder.X += Random.NewFloat(-4, 4);
Holder.Y += Random.NewFloat(-4, 4);
Holder.GetItem().Generate();
this.Area.Add(Holder);
LevelSave.Add(Holder);
Holder.RandomVelocity();
}
}
}
}
}
if (this.Freezed) {
this.Velocity.X = 0;
this.Velocity.Y = 0;
this.Acceleration.X = 0;
this.Acceleration.Y = 0;
this.Knockback.X = 0;
this.Knockback.Y = 0;
if (this.Body != null) {
this.Body.SetTransform(this.X, this.Y, 0);
}
return;
}
if (this.Room != null && !this.IgnoreRooms) {
this.Room.NumEnemies += 1;
}
if (this.Dead || this.Dd) {
return;
}
if (this.Ai == null) {
this.Ai = this.GetAiWithLow(this.State);
if (this.Ai != null) {
this.Ai.Self = this;
try {
this.Ai.OnEnter();
} catch (RuntimeException) {
E.PrintStackTrace();
Log.Error("AI error in " + this.GetClass().GetSimpleName());
this.Become("idle");
}
}
}
if (this.Ai != null) {
try {
this.Ai.Update(Dt * SpeedMod);
} catch (RuntimeException) {
E.PrintStackTrace();
Log.Error("AI error in " + this.GetClass().GetSimpleName());
this.Become("idle");
}
if (this.Ai != null) {
this.Ai.T += Dt * SpeedMod;
}
}
this.FindTarget(false);
if (this.Target != null && (this.Target.Invisible || this.Target.IsDead())) {
this.Target = null;
this.Become("idle");
}
if (!this.Friendly && !this.Dd) {
foreach (Player Player in Colliding) {
Player.ModifyHp(-1, this, false);
}
}
}
protected override void Common() {
float Dt = GetDt();
DoVel();
this.T += Dt;
this.Timer += Dt;
this.Invt = Math.Max(0, this.Invt - Dt);
this.Invtt = Math.Max(0, this.Invtt - Dt);
if (!this.Dead && !(this is Boss || this is Fly)) {
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
if (IgnoreVel) {
this.X = this.Body.GetPosition().X;
this.Y = this.Body.GetPosition().Y;
} else {
this.Body.SetTransform(this.X, this.Y + this.Z, 0);
this.Lz = this.Z;
this.Body.SetLinearVelocity(this.Velocity.X * SpeedMod + this.Knockback.X, this.Velocity.Y * SpeedMod + this.Knockback.Y);
}
}
}
public override void OnCollision(Entity Entity) {
if (Entity is Player) {
Player Player = (Player) Entity;
if (Player.IsDead()) {
return;
}
this.Target = Player;
if (!Friendly && !this.State.Equals("defeated") && !Dead) {
if (Player.GetInvt() == 0) {
Player.KnockBackFrom(this, 2);
}
Player.ModifyHp(-1, this, false);
}
this.Colliding.Add(Player);
}
}
public override void OnCollisionEnd(Entity Entity) {
if (Entity is Player) {
Player Player = (Player) Entity;
if (Player.IsDead()) {
return;
}
this.Colliding.Remove(Player);
}
}
protected List GetDrops<Item> () {
List<Item> Items = new List<>();
if (Random.Chance(30)) {
Gold Gold = new Gold();
Gold.Generate();
Items.Add(Gold);
}
if (Random.Chance(10)) {
Items.Add(new KeyC());
}
return Items;
}
public override HpFx ModifyHp(int Amount, Creature From) {
if (this.Dd) {
return null;
}
return base.ModifyHp(Amount, From);
}
protected void DeathEffects() {
this.Done = true;
Drop = true;
if (this.Room != null) {
this.Room.NumEnemies -= 1;
}
All.Remove(this);
Every.Remove(this);
}
protected override void Die(bool Force) {
if (Dd) {
return;
}
Camera.Shake(3);
this.Dd = true;
this.Done = false;
this.Velocity.X = 0;
this.Velocity.Y = 0;
Tween.To(new Tween.Task(0.7f, 0.2f) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(1.5f, 0.1f) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
public override void OnEnd() {
Dead = true;
Remove = true;
DeathEffects();
Camera.Shake(3);
if (!Force) {
GameSave.KillCount++;
if (GameSave.KillCount >= 10) {
Achievements.Unlock(Achievements.UNLOCK_BLACK_HEART);
}
if (GameSave.KillCount >= 100) {
Achievements.Unlock(Achievements.UNLOCK_BLOOD_CROWN);
}
}
if (Player.Instance != null && !Player.Instance.IsDead() && !Force && Random.Chance(20) && !NoLoot) {
HeartFx Fx = new HeartFx();
Fx.X = X + W / 2 + Random.NewFloat(-4, 4);
Fx.Y = Y + H / 2 + Random.NewFloat(-4, 4);
Dungeon.Area.Add(Fx);
LevelSave.Add(Fx);
Fx.RandomVelocity();
}
}
}).Delay(0.2f);
Tween.To(new Tween.Task(0.5f, 0.1f) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
}).Delay(0.2f);
}
});
Tween.To(new Tween.Task(1.3f, 0.2f) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
});
}
protected override float GetDt() {
return base.GetDt() * SpeedMod;
}
protected float MoveToPoint(float X, float Y, float Speed) {
Speed *= 0.8f;
float Dx = X - this.X - this.W / 2;
float Dy = Y - this.Y - this.H / 2;
float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
this.Acceleration.X += Dx / D * Speed;
this.Acceleration.Y += Dy / D * Speed;
return D;
}
public override void Become(string State) {
if (!this.State.Equals(State)) {
if (this.Ai != null) {
try {
this.Ai.OnExit();
} catch (RuntimeException) {
E.PrintStackTrace();
Log.Error("Mob AI exdsception " + this.GetClass().GetSimpleName());
}
}
this.Ai = this.GetAiWithLow(State);
if (this.Ai != null) {
this.Ai.Self = this;
try {
this.Ai.OnEnter();
} catch (RuntimeException) {
E.PrintStackTrace();
Log.Error("Mob AI exception " + this.GetClass().GetSimpleName());
}
} else {
Log.Error("'" + State + "' ai is not found for mob " + this.GetClass().GetSimpleName());
}
}
base.Become(State);
}
protected State GetAiWithLow(string State) {
return GetAi(State);
}
protected State GetAi(string State) {
switch (State) {
case "getout": {
return new GetOutState();
}
}
return null;
}
public void FindTarget(bool Force) {
if (this.Target == null && Dungeon.Level != null) {
foreach (Creature Player in (this.Stupid ? Mob.All : Player.All)) {
if (Player.Invisible || Player == this) {
continue;
}
this.Target = Player;
}
}
}
protected override void OnHurt(int A, Entity From) {
base.OnHurt(A, From);
if (From is Player) {
this.Target = (Creature) From;
}
}
public override bool RollBlock() {
return false;
}
public override Point GetAim() {
if (this.Target != null) {
return new Point(Player.Instance.X + 8, Player.Instance.Y + 8);
} else {
return new Point(this.X + this.W, this.Y);
}
}
public override void Destroy() {
base.Destroy();
All.Remove(this);
Every.Remove(this);
}
public override int HashCode() {
return Id;
}
public Mob() {
_Init();
}
}
}