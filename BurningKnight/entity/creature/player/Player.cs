using BurningKnight.entity.creature.inventory;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.level.entities;
using BurningKnight.game;
using BurningKnight.physics;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature {
		public static Type ToSet = Type.NONE;
		public static Item StartingItem;
		public static float MobSpawnModifier = 1f;
		public static List<Player> All = new List<>();
		public static Player Instance;
		public static Entity Ladder;
		public static ShaderProgram Shader;
		public static bool ShowStats;
		public static string HatId;
		public static string Skin;
		public static TextureRegion Balloon = Graphics.GetTexture("item-red_balloon");
		public static bool Sucked = false;
		public static bool DullDamage;
		private static Dictionary<string, Animation> Skins = new Dictionary<>();
		private static Animation HeadAnimations = Animation.Make("actor-gobbo", "-gobbo");
		private static AnimationData HeadIdle = HeadAnimations.Get("idle");
		private static AnimationData HeadRun = HeadAnimations.Get("run");
		private static AnimationData HeadHurt = HeadAnimations.Get("hurt");
		private static AnimationData HeadRoll = HeadAnimations.Get("roll");
		private static TextureRegion Wing = Graphics.GetTexture("item-half_wing");
		private static TextureRegion PlayerTexture = Graphics.GetTexture("props-gobbo_full");
		private static int[] Offsets = {0, 0, 0, -1, -1, -1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
		public float Accuracy;
		public float Al;
		private AnimationData Animation;
		private int Bombs;
		public int BurnLevel;
		public bool DrawInvt;
		private float Fa;
		public int FireResist;
		public int Flight;
		public int FrostLevel;
		public float GoldModifier = 1f;
		private bool GotHit;
		private bool HadEnemies;
		public bool HasBkKey;
		public bool HasRedLine;
		private TextureRegion Hat;
		public float Heat;
		private List<ItemHolder> Holders = new List<>();
		private AnimationData Hurt;
		private AnimationData Idle;
		private Inventory Inventory;
		private int Keys;
		private AnimationData Killed;
		private float Last;
		private float LastBlood;
		private float LastFx = 0;
		private Vector2 LastGround = new Vector2();
		public int LavaResist;
		public bool LeaveSmall;
		public int LeaveVenom;
		protected int Level;
		private PointLight Light;
		protected float Mana;
		protected int ManaMax;
		private int Money;
		private bool Moved;
		private string Name;
		public int NumCollectedHearts;
		private int NumGoldenHearts;
		private int NumIronHearts;
		private bool OnGround;
		public Vector2 OrbitalRing = new Vector2();
		public ItemPickupFx PickupFx;
		public int PoisonResist;
		private AnimationData Roll;
		private bool Rolled;
		private bool Rolling;
		public bool Rotating;
		private AnimationData Run;
		public int Sales;
		public bool SeePath;
		public bool SeeSecrets;
		public int Step;
		public float StopT;
		public int StunResist;
		private float Sx = 1f;
		private float Sy = 1f;
		private bool Teleport;
		public bool ToDeath;
		public float Tt;
		public Type Type;
		public UiInventory Ui;
		private bool WasFreezed;
		private bool WasPoisoned;
		private float Zvel;

		static Player() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/rainbow.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());
		}

		public Player() {
			_Init();
			this("player");
		}

		public Player(string Name) {
			_Init();
			if (Instance != null) {
				Instance.Done = true;
				Instance.Destroy();
			}

			All.Add(this);
			Instance = this;
			Ui.Ui.Dead = false;
		}

		protected void _Init() {
			{
				Defense = 1;
			}

			{
				HpMax = 6;
				ManaMax = 8;
				Level = 1;
				Mul = 0.7f;
				Speed = 25;
				AlwaysActive = true;
				InvTime = 1f;
				SetSkin("body");
			}
		}

		public override void RenderBuffs() {
			base.RenderBuffs();
			Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			Item Item = Inventory.GetSlot(Inventory.Active);

			if (Item is Gun) ((Gun) Item).RenderReload();

			if (BurningKnight.Instance != null && BurningKnight.Instance.Rage && Exit.Instance != null) {
				var Dx = Exit.Instance.X + 8 - X - W / 2;
				var Dy = Exit.Instance.Y + 8 - Y - H / 2;
				var A = (float) Math.Atan2(Dy, Dx);
				Graphics.Batch.End();
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
				var An = (float) Math.ToRadians(10);
				var D = 28 + Math.Cos(Dungeon.Time * 6) * 2.5f;
				var D2 = D + 8;
				Graphics.Shape.SetColor(0, 0, 0, 1);
				Graphics.Shape.RectLine(X + W / 2 + Math.Cos(A - An) * D, Y + H / 2 + Math.Sin(A - An) * D, X + W / 2 + Math.Cos(A) * D2, Y + H / 2 + Math.Sin(A) * D2, 4f);
				Graphics.Shape.RectLine(X + W / 2 + Math.Cos(A + An) * D, Y + H / 2 + Math.Sin(A + An) * D, X + W / 2 + Math.Cos(A) * D2, Y + H / 2 + Math.Sin(A) * D2, 4f);
				var V = Math.Sin(Dungeon.Time * 12) * 0.5f + 0.5f;
				Graphics.Shape.SetColor(1, V, V, 1);
				Graphics.Shape.RectLine(X + W / 2 + Math.Cos(A - An) * D, Y + H / 2 + Math.Sin(A - An) * D, X + W / 2 + Math.Cos(A) * D2, Y + H / 2 + Math.Sin(A) * D2, 2);
				Graphics.Shape.RectLine(X + W / 2 + Math.Cos(A + An) * D, Y + H / 2 + Math.Sin(A + An) * D, X + W / 2 + Math.Cos(A) * D2, Y + H / 2 + Math.Sin(A) * D2, 2);
				Graphics.Shape.End();
				Graphics.Batch.Begin();
			}

			if (Dungeon.Depth < 0) return;

			var Count = 0;
			Mob Last = null;

			foreach (Mob Mob in Mob.All)
				if (Mob.Room == Room) {
					Last = Mob;
					Count++;
				}

			if (Last != null && Count == 1 && !Ui.HideUi) {
				var Dx = Last.X + Last.W / 2 - this.X - W / 2;
				var Dy = Last.Y + Last.H / 2 - this.Y - H / 2;
				var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
				var A = GetAngleTo(Last.X + Last.W / 2, Last.Y + Last.H / 2);

				if (D < 48) return;

				D -= 32;
				float Cx = Camera.Game.Position.X;
				float Cy = Camera.Game.Position.Y;
				var X = MathUtils.Clamp(Cx - Display.GAME_WIDTH / 2 + 16, Cx + Display.GAME_WIDTH / 2 - 16, (float) Math.Cos(A) * D + this.X + W / 2);
				var Y = MathUtils.Clamp(Cy - Display.GAME_HEIGHT / 2 + 16, Cy + Display.GAME_HEIGHT / 2 - 16, (float) Math.Sin(A) * D + this.Y + H / 2);
				Graphics.StartAlphaShape();
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Shape.SetColor(1, 0.1f, 0.1f, 0.8f);
				A = (float) Math.Atan2(Y - Last.Y - Last.W / 2, X - Last.X - Last.H / 2);
				float M = 10;
				var Am = 0.5f;
				Graphics.Shape.RectLine(X, Y, X + (float) Math.Cos(A - Am) * M, Y + (float) Math.Sin(A - Am) * M, 2);
				Graphics.Shape.RectLine(X, Y, X + (float) Math.Cos(A + Am) * M, Y + (float) Math.Sin(A + Am) * M, 2);
				Graphics.EndAlphaShape();
			}
		}

		public static float GetStaticMage() {
			return Instance == null ? (ToSet == Type.WIZARD ? 1f : 0.1f) : Instance.GetMage();
		}

		public float GetMage() {
			return this.Type == Type.WIZARD ? 1f : 0.1f;
		}

		public static float GetStaticWarrior() {
			return Instance == null ? (ToSet == Type.WARRIOR ? 1f : 0.1f) : Instance.GetWarrior();
		}

		public float GetWarrior() {
			return this.Type == Type.WARRIOR ? 1f : 0.1f;
		}

		public override void SetHpMax(int HpMax) {
			base.SetHpMax(HpMax);

			if (this.HpMax >= 16) Achievements.Unlock(Achievements.GET_8_HEART_CONTAINERS);
		}

		public static float GetStaticRanger() {
			return Instance == null ? (ToSet == Type.RANGER ? 1f : 0.1f) : Instance.GetRanger();
		}

		public float GetRanger() {
			return this.Type == Type.RANGER ? 1f : 0.1f;
		}

		public int GetKeys() {
			return Keys;
		}

		public void SetKeys(int Money) {
			Keys = Math.Min(99, Money);
		}

		public void ResetHit() {
			GotHit = false;
		}

		public int GetBombs() {
			return Bombs;
		}

		public void SetBombs(int Money) {
			Bombs = Math.Min(99, Money);
		}

		public int GetMoney() {
			return Money;
		}

		public void SetMoney(int Money) {
			this.Money = Money;

			if (Money >= 100) Achievements.Unlock(Achievements.UNLOCK_MONEY_PRINTER);

			if (Money >= 300) Achievements.Unlock(Achievements.COLLECT_300_GOLD);
		}

		public Type GetType() {
			return this.Type;
		}

		public void SetType(Type Type) {
			this.Type = Type;
		}

		public override void RenderShadow() {
			var Z = this.Z;
			var Flying = false;
			Graphics.Shadow(this.X + Hx, this.Y - (Flying ? 3 : 0), Hw, Hh, Z);
		}

		public void SetSkin(string Add) {
			Animation Animations;
			Skin = Add;

			if (!Add.IsEmpty()) Add = "-" + Add;

			if (Skins.ContainsKey(Add)) {
				Animations = Skins.Get(Add);
			}
			else {
				Animations = Animation.Make("actor-gobbo", Add);
				Skins.Put(Add, Animations);
			}


			Idle = Animations.Get("idle");
			Run = Animations.Get("run");
			Hurt = Animations.Get("hurt");
			Roll = Animations.Get("roll");
			Killed = Animations.Get("dead");
			Animation = Idle;
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);

			if (UiMap.Instance != null) UiMap.Instance.Remove();

			HasBkKey = false;
			All.Remove(this);
		}

		public string GetName() {
			return Name;
		}

		public void SetName(string Name) {
			this.Name = Name;
		}

		public override void Init() {
			base.Init();
			Invt = 0.5f;
			Al = 0;
			Rotating = false;
			Tween.To(new Tween.Task(0, 0.1f) {

		public override void OnEnd() {
			base.OnEnd();
			Camera.Follow(Instance, true);
		}

		private enum Type {
			WARRIOR,
			WIZARD,
			RANGER,
			NONE
		}
	});

	T = 0;
	if (ToSet != Type.NONE) {
	this.Type = ToSet;
	ToSet = Type.NONE;
} else if (this.Type == null) {
this.Type = Type.WARRIOR;
}
if (Instance == null) {
Instance = this;
}
this.Mana = this.ManaMax;
this.Inventory = new Inventory(this);
this.Body = this.CreateSimpleBody(3, 0, 10, 11, BodyDef.BodyType.DynamicBody, false);
DoTp(true);
switch (this.Type) {
case WARRIOR:
case WIZARD: {
this.Accuracy -= 5;
break;
}
}
Light = World.NewLight(256, new Color(1, 1, 1, 1f), 180, X, Y);
Light.SetPosition(this.X + 8, this.Y + 8);
Light.AttachToBody(this.Body, 8, 8, 0);
Light.SetIgnoreAttachedBody(true);
if (Dungeon.Depth == -3) {
this.Inventory.Clear();
this.HpMax = 10;
this.Hp = 10;
this.Give(new Sword());
Player.Instance.Tp(Spawn.Instance.X, Spawn.Instance.Y);
}
Camera.Follow(this, true);
}
public void Generate() {
this.Inventory.Clear();
Bombs = 1;
Keys = 0;
Money = 0;
if (Dungeon.Depth == -3) {
this.HpMax = 12;
this.Hp = 12;
this.Give(new Sword());
} else {
if (StartingItem != null) {
this.Give(StartingItem);
StartingItem = null;
} else {
this.Give(new Sword());
}
if (this.Type != Type.WIZARD) {
this.ManaMax -= 2;
this.Mana -= 2;
}
if (this.Type == Type.RANGER) {
this.HpMax = 4;
this.Hp = 4;
}
}
if (HatId != null) {
this.Give(ItemPickupFx.SetSkin(HatId));
} else {
string Id = GlobalSave.GetString("last_hat", null);
this.SetHat(Id);
}
if (Random.GetSeed().Equals("HP")) {
this.HpMax = 12;
this.Hp = 12;
} else if (Random.GetSeed().Equals("DIE")) {
this.HpMax = 2;
this.Hp = 2;
} else if (Random.GetSeed().Equals("BOMB")) {
this.Bombs = 99;
} else if (Random.GetSeed().Equals("KEY")) {
this.Keys = 99;
} else if (Random.GetSeed().Equals("GOLD")) {
this.Money = 999;
}
}
public void Give(Item Item) {
if (Item is Hat) {
this.Inventory.SetSlot(3, Item);
Item.SetOwner(this);
((Accessory) Item).OnEquip(false);
} else {
this.Inventory.Add(new ItemHolder(Item));
}
}
private void DoTp(bool FromInit) {
if (this.Teleport) {
this.Tp(this.LastGround.X, this.LastGround.Y);
return;
}
if (Dungeon.Depth == -3) {
} else if (Dungeon.Depth == -1) {
Room Room = Dungeon.Level.GetRooms().Get(0);
this.Tp((Room.Left + Room.GetWidth() / 2) * 16 - 8, Room.Top * 16 + 32);
} else if (Ladder != null && (Dungeon.LoadType != Entrance.LoadType.LOADING || (!FromInit && (Dungeon.Level.FindRoomFor(this.X + this.W / 2, this.Y) == null)))) {
this.Tp(Ladder.X, Ladder.Y - 4);
} else if (Ladder == null) {
Log.Error("Null lader!");
}
Vector3 Vec = Camera.Game.Project(new Vector3(Player.Instance.X + Player.Instance.W / 2, Player.Instance.Y + Player.Instance.H / 2, 0));
Vec = Camera.Ui.Unproject(Vec);
Vec.Y = Display.GAME_HEIGHT - Vec.Y / Display.UI_SCALE;
Dungeon.DarkX = Vec.X / Display.UI_SCALE;
Dungeon.DarkY = Vec.Y;
}
public void SetHat(string Name) {
if (Name == null || Name.IsEmpty()) {
Hat = null;
return;
}
GlobalSave.Put("last_hat", Name);
HatId = Name;
this.Hat = Graphics.GetTexture("hat-" + Name + "-idle-00");
}
public override void Tp(float X, float Y) {
base.Tp(X, Y);
Camera.Follow(this, true);
OrbitalRing.X = this.X + this.W / 2;
OrbitalRing.Y = this.Y + this.H / 2;
}
public void AddIronHearts(int A) {
NumIronHearts += A;
}
public void AddGoldenHearts(int A) {
NumGoldenHearts += A;
}
public int GetIronHearts() {
return NumIronHearts;
}
public int GetGoldenHearts() {
return NumGoldenHearts;
}
public void SetUi(UiInventory Ui) {
this.Ui = Ui;
}
public override bool IsUnhittable() {
return base.IsUnhittable() || this.Rolling;
}
public void ModifyManaMax(int A) {
this.ManaMax += A;
this.ModifyMana(0);
}
public void ModifyMana(int A) {
this.Mana = (int) MathUtils.Clamp(0, this.ManaMax, this.Mana + A);
}
public override void Render() {
Graphics.Batch.SetColor(1, 1, 1, this.A);
float Offset = 0;
if (this.Rotating) {
this.Al += Gdx.Graphics.GetDeltaTime() * 960;
Graphics.Render(PlayerTexture, this.X + 6.5f, this.Y + 2.5f, this.Al, 6.5f, 2.5f, false, false);
Graphics.Batch.SetColor(1, 1, 1, 1);
} else {
if (this.Rolling) {
this.Animation = Roll;
} else if (this.Invt > 0) {
this.Animation = Hurt;
Hurt.SetFrame(0);
} else if (!this.IsFlying() && this.State.Equals("run")) {
this.Animation = Run;
} else {
this.Animation = Idle;
}
if (this.Invtt == 0) {
this.DrawInvt = false;
}
int Id = this.Animation.GetFrame();
float Of = Offsets[Id] - 2;
if (this.Invt > 0) {
Id += 16;
} else if (!this.IsFlying() && this.State.Equals("run")) {
Id += 8;
}
if (this.Ui != null && !IsRolling()) {
this.Ui.RenderBeforePlayer(this, Of);
}
bool Shade = (this.DrawInvt && this.Invtt > 0) || (Invt > 0 && Invt % 0.2f > 0.1f);
TextureRegion Region = this.Animation.GetCurrent().Frame;
if (Shade) {
Texture Texture = Region.GetTexture();
Graphics.Batch.End();
Shader.Begin();
Shader.SetUniformf("time", Dungeon.Time);
Shader.SetUniformf("pos", new Vector2((float) Region.GetRegionX() / Texture.GetWidth(), (float) Region.GetRegionY() / Texture.GetHeight()));
Shader.SetUniformf("size", new Vector2((float) Region.GetRegionWidth() / Texture.GetWidth(), (float) Region.GetRegionHeight() / Texture.GetHeight()));
Shader.SetUniformf("a", this.A);
Shader.SetUniformf("white", Invt > 0 ? 1 : 0);
Shader.End();
Graphics.Batch.SetShader(Shader);
Graphics.Batch.Begin();
} else if (this.Fa > 0) {
Graphics.Batch.End();
Mob.Frozen.Begin();
Mob.Frozen.SetUniformf("time", Dungeon.Time);
Mob.Frozen.SetUniformf("f", this.Fa);
Mob.Frozen.SetUniformf("a", this.A);
Mob.Frozen.SetUniformf("freezed", this.WasFreezed ? 1f : 0f);
Mob.Frozen.SetUniformf("poisoned", this.WasPoisoned ? 1f : 0f);
Mob.Frozen.End();
Graphics.Batch.SetShader(Mob.Frozen);
Graphics.Batch.Begin();
}
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
float Angle = 0;
this.Animation.Render(this.X - Region.GetRegionWidth() / 2 + 8, this.Y + this.Z + Offset, false, false, Region.GetRegionWidth() / 2, 0, Angle, this.Sx * (this.Flipped ? -1 : 1), this.Sy);
if (this.Hat != null && !this.IsRolling()) {
Graphics.Render(this.Hat, this.X + W / 2 - (this.Flipped ? -1 : 1) * 7, this.Y + 1 + this.Z + Offsets[Id] + Region.GetRegionHeight() / 2 - 2 + Offset, Angle, Region.GetRegionWidth() / 2, 0, false, false, this.Sx * (this.Flipped ? -1 : 1),
this.Sy);
} else {
AnimationData Anim = HeadIdle;
if (this.Rolling) {
Anim = HeadRoll;
} else if (this.Invt > 0) {
Anim = HeadHurt;
} else if (this.State.Equals("run") && !this.IsFlying()) {
Anim = HeadRun;
}
Anim.SetFrame(this.Animation.GetFrame());
Region = Anim.GetCurrent().Frame;
Anim.Render(this.X - Region.GetRegionWidth() / 2 + 8, this.Y + this.Z + Offset, false, false, Region.GetRegionWidth() / 2, 0, Angle, this.Sx * (this.Flipped ? -1 : 1), this.Sy);
}
Graphics.Batch.SetColor(1, 1, 1, 1);
if (Shade || this.Fa > 0) {
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
Graphics.Batch.Begin();
}
if (!this.Rolling && this.Ui != null && Dungeon.Depth != -2) {
this.Ui.RenderOnPlayer(this, Of + Offset);
}
}
Graphics.Batch.SetColor(1, 1, 1, 1);
}
public bool IsRolling() {
return this.Rolling;
}
public override void OnCollision(Entity Entity) {
if (Entity is ItemHolder) {
ItemHolder Item = (ItemHolder) Entity;
if (Item.GetItem() is Coin) {
Item.Remove();
Item.Done = true;
Tween.To(new Tween.Task(20, 0.2f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Ui.Y;
}
public override void SetValue(float Value) {
Ui.Y = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.1f) {
public override void OnEnd() {
GlobalSave.Put("num_coins", GlobalSave.GetInt("num_coins") + 1);
}
}).Delay(0.5f);
Tween.To(new Tween.Task(0, 0.2f) {
public override float GetValue() {
return Ui.Y;
}
public override void SetValue(float Value) {
Ui.Y = Value;
}
public override void OnStart() {
if (Ui.Y < 20) {
DeleteSelf();
}
}
}).Delay(3.1f);
}
});
for (int I = 0; I < 10; I++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
} else if (!Item.GetItem().Shop && (Item.GetItem().HasAutoPickup() || Item.Auto)) {
if (this.TryToPickup(Item) && !Item.Auto) {
if (!(Item.GetItem() is Gold)) {
this.Area.Add(new ItemPickedFx(Item));
}
Item.Done = true;
Item.Remove();
}
} else if (!Item.Falling) {
this.Holders.Add(Item);
if (this.PickupFx == null) {
this.PickupFx = new ItemPickupFx(Item, this);
this.Area.Add(this.PickupFx);
}
}
} else if (Entity is Mob) {
if (this.FrostLevel > 0) {
((Mob) Entity).AddBuff(new FrozenBuff());
}
if (this.BurnLevel > 0) {
((Mob) Entity).AddBuff(new BurningBuff());
}
}
}
public override void OnCollisionEnd(Entity Entity) {
if (Entity is ItemHolder) {
if (this.PickupFx != null) {
this.PickupFx.Remove();
this.PickupFx = null;
}
this.Holders.Remove(Entity);
if (this.Holders.Size() > 0 && !Ui.HideUi) {
this.PickupFx = new ItemPickupFx(this.Holders.Get(0), this);
this.Area.Add(this.PickupFx);
}
}
}
public bool TryToPickup(ItemHolder Item) {
if (!Item.Done) {
if (Item.GetItem() is Bomb && !(Item.GetItem() is InfiniteBomb)) {
SetBombs(Bombs + Item.GetItem().GetCount());
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else if (Item.GetItem() is Gold) {
SetMoney(Money + Item.GetItem().GetCount());
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else if (Item.GetItem() is Key && !(Item.GetItem() is BurningKey)) {
SetKeys(Keys + Item.GetItem().GetCount());
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else if (Item.GetItem() is WeaponBase) {
if (Inventory.IsEmpty(0)) {
Inventory.SetSlot(0, Item.GetItem());
Item.GetItem().SetOwner(this);
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else if (Inventory.IsEmpty(1)) {
Inventory.SetSlot(1, Item.GetItem());
Item.GetItem().SetOwner(this);
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else {
Item It = Item.GetItem();
Item.SetItem(this.Inventory.GetSlot(this.Inventory.Active));
this.Inventory.SetSlot(this.Inventory.Active, It);
It.SetOwner(this);
It.OnPickup();
this.PlaySfx("pickup_item");
return false;
}
} else if (Item.GetItem() is ActiveItem || (Item.GetItem() is Consumable && !(Item.GetItem() is Autouse))) {
if (Inventory.GetSlot(2) == null) {
Inventory.SetSlot(2, Item.GetItem());
Item.GetItem().SetOwner(this);
Item.GetItem().OnPickup();
Item.Remove();
Item.Done = true;
this.PlaySfx("pickup_item");
for (int J = 0; J < 3; J++) {
PoofFx Fx = new PoofFx();
Fx.X = Item.X + Item.W / 2;
Fx.Y = Item.Y + Item.H / 2;
Dungeon.Area.Add(Fx);
}
return true;
} else {
Item It = Item.GetItem();
Item.SetItem(this.Inventory.GetSlot(2));
this.Inventory.SetSlot(2, It);
It.SetOwner(this);
It.OnPickup();
this.PlaySfx("pickup_item");
return false;
}
} else {
Item It = Item.GetItem();
It.SetOwner(this);
It.OnPickup();
this.PlaySfx("pickup_item");
Inventory.Add(Item);
return true;
}
}
return false;
}
public bool DidGetHit() {
return GotHit;
}
protected override void Common() {
base.Common();
}
public int GetManaMax() {
return this.ManaMax;
}
public Inventory GetInventory() {
return this.Inventory;
}
public override void Update(float Dt) {
base.Update(Dt);
Light.SetActive(true);
Light.AttachToBody(Body, 8, 8, 0);
Light.SetPosition(X + 8, Y + 8);
Light.SetDistance(180);
if (this.HasBuff(Buffs.BURNING)) {
this.Light.SetColor(1, 0.5f, 0f, 1);
} else {
this.Light.SetColor(1, 1, 0.8f, 1);
}
if (!this.Rolling) {
if (this.IsFlying() || this.Touches[Terrain.WALL] || this.Touches[Terrain.FLOOR_A] || this.Touches[Terrain.FLOOR_B] || this.Touches[Terrain.FLOOR_C] || this.Touches[Terrain.FLOOR_D] || this.Touches[Terrain.DISCO]) {
this.OnGround = true;
this.LastGround.X = this.X;
this.LastGround.Y = this.Y;
}
if (!this.OnGround) {
this.Teleport = true;
for (int I = 0; I < 5; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Dungeon.Area.Add(Fx);
}
this.DoTp(false);
for (int I = 0; I < 5; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Dungeon.Area.Add(Fx);
}
this.Teleport = false;
this.ModifyHp(-1, null, true);
}
this.OnGround = false;
}
this.Z = Math.Max(0, this.Zvel * Dt + this.Z);
this.Zvel = this.Zvel - Dt * 220;
OrbitalRing.Lerp(new Vector2(this.X + this.W / 2, this.Y + this.H / 2), 4 * Dt);
if (this.ToDeath) {
this.T += Dt;
this.Animation.Update(Dt * (this.Flipped != this.Acceleration.X < 0 && this.Animation == Run ? -1 : 1));
if (this.T >= 1f) {
Ui.Ui.Dead = true;
base.Die(false);
this.Dead = true;
this.Done = true;
Camera.Shake(10);
this.Remove();
DeathEffect(Killed);
BloodFx.Add(this, 20);
List<Item> Items = new List<>();
for (int I = 0; I < 3; I++) {
if (Inventory.GetSlot(I) != null) {
Items.Add(Inventory.GetSlot(I));
}
}
for (int I = 0; I < Inventory.GetSpace(); I++) {
Items.Add(Inventory.GetSpace(I));
}
foreach (Item Item in Items) {
ItemHolder Holder = new ItemHolder();
Holder.X = this.X + Random.NewFloat(16);
Holder.Y = this.Y + Random.NewFloat(16);
Holder.RandomVelocity();
Holder.SetItem(Item);
Dungeon.Area.Add(Holder);
}
SaveManager.Delete();
Inventory.Clear();
}
return;
}
if (this.Mana != this.ManaMax) {
bool Dark = Player.Instance.IsDead();
if (!Dark) {
Dark = Boss.All.Size() > 0 && Player.Instance.Room is BossRoom && !BurningKnight.Instance.Rage;
if (!Dark) {
foreach (Mob Mob in Mob.All) {
if (Mob.Room == Player.Instance.Room) {
Dark = true;
break;
}
}
}
}
}
if (this.Dead) {
base.Common();
return;
}
if (this.Hp <= 2) {
this.Last += Dt;
this.LastBlood += Dt;
if (this.LastBlood > 0.1f) {
this.LastBlood = 0;
BloodDropFx Fx = new BloodDropFx();
Fx.Owner = this;
Dungeon.Area.Add(Fx);
}
if (this.Last >= 1f && Settings.Blood) {
this.Last = 0;
BloodSplatFx Fxx = new BloodSplatFx();
Fxx.X = X + Random.NewFloat(W) - 8;
Fxx.Y = Y + Random.NewFloat(H) - 8;
Dungeon.Area.Add(Fxx);
}
}
Item Item = this.Inventory.GetSlot(this.Inventory.Active);
if (Item != null) {
Item.UpdateInHands(Dt);
}
this.Heat = Math.Max(0, this.Heat - Dt / 3);
if (!Sucked && Dialog.Active == null && !this.Freezed && !UiMap.Large) {
if (!this.Rolling) {
if (Input.Instance.IsDown("left")) {
this.Acceleration.X -= this.Speed;
}
if (Input.Instance.IsDown("right")) {
this.Acceleration.X += this.Speed;
}
if (Input.Instance.IsDown("up")) {
this.Acceleration.Y += this.Speed;
}
if (Input.Instance.IsDown("down")) {
this.Acceleration.Y -= this.Speed;
}
Vector2 Move = Input.Instance.GetAxis("move");
if (Move.Len2() > 0.2f) {
this.Acceleration.X += Move.X * this.Speed;
this.Acceleration.Y -= Move.Y * this.Speed;
}
}
if (!this.Rolling) {
if (Input.Instance.WasPressed("roll")) {
Rolled = true;
float F = 80;
IgnoreAcceleration = true;
if (Acceleration.Len() > 1f) {
double A = (Math.Atan2(Acceleration.Y, Acceleration.X));
Acceleration.X = (float) Math.Cos(A) * Speed * F;
Acceleration.Y = (float) Math.Sin(A) * Speed * F;
} else {
double A = (GetAngleTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y));
Acceleration.X = (float) Math.Cos(A) * Speed * F;
Acceleration.Y = (float) Math.Sin(A) * Speed * F;
}
for (int I = 0; I < 3; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Fx.T = 0.5f;
Dungeon.Area.Add(Fx);
}
PlaySfx("dash_short");
Player Self = this;
Tween.To(new Tween.Task(0, 0.2f) {
public override void OnEnd() {
RemoveBuff(Buffs.BURNING);
IgnoreAcceleration = false;
Self.Velocity.X = 0;
Self.Velocity.Y = 0;
Tween.To(new Tween.Task(0, 0) {
public override void OnStart() {
Rolling = false;
}
public override void OnEnd() {
base.OnEnd();
Animation = Idle;
}
}).Delay(0.05f);
}
}).Delay(0.05f);
this.Rolling = true;
this.Velocity.X = 0;
this.Velocity.Y = 0;
}
}
} else if (Dialog.Active != null) {
if (Input.Instance.WasPressed("interact")) {
Dialog.Active.Skip();
}
}
float V = this.Acceleration.Len2();
if (Knockback.Len() + V > 4f) {
this.StopT = 0;
} else {
StopT += Dt;
}
if (V > 20) {
this.Become("run");
} else {
this.Become("idle");
}
base.Common();
if (this.Animation != null && !this.Freezed) {
if (this.Animation.Update(Dt)) {
}
}
if (this.IsRolling()) {
LastFx += Dt;
if (LastFx >= 0.05f) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Fx.T = 0.5f;
Dungeon.Area.Add(Fx);
LastFx = 0;
}
}
if (!this.Freezed) {
float Dx = this.X + this.W / 2 - Input.Instance.WorldMouse.X;
this.Flipped = Dx >= 0;
}
int I = Level.ToIndex(Math.Round((this.X) / 16), Math.Round((this.Y + this.H / 2) / 16));
if (this.BurnLevel > 0) {
Dungeon.Level.SetOnFire(I, true);
}
if (this.FrostLevel > 0) {
Dungeon.Level.Freeze(I);
if (this.FrostLevel >= 4) {
if (Dungeon.Level.LiquidData[I] == Terrain.LAVA) {
Dungeon.Level.Set(I, Terrain.ICE);
Dungeon.Level.UpdateTile(Level.ToX(I), Level.ToY(I));
}
}
}
}
public int GetMana() {
return (int) this.Mana;
}
public int GetLevel() {
return this.Level;
}
public static Type GetTypeFromId(int Id) {
switch (Id) {
case 0: {
return Type.WARRIOR;
}
case 1: {
return Type.WIZARD;
}
case 2: {
return Type.RANGER;
}
default:{
return Type.NONE;
}
}
}
public static int GetTypeId(Type Type) {
switch (Type) {
case WARRIOR: {
return 0;
}
case WIZARD: {
return 1;
}
case RANGER: {
return 2;
}
default:{
return 3;
}
}
}
public override bool IsFlying() {
return Flight > 0 || this.Rolling;
}
protected override void OnTouch(short T, int X, int Y, int Info) {
if (T == Terrain.WATER && !this.IsFlying()) {
if (this.HasBuff(Buffs.BURNING)) {
int Num = GlobalSave.GetInt("num_fire_out") + 1;
GlobalSave.Put("num_fire_out", Num);
if (Num >= 50) {
Achievements.Unlock(Achievements.UNLOCK_WATER_BOLT);
}
this.RemoveBuff(Buffs.BURNING);
for (int I = 0; I < 20; I++) {
SteamFx Fx = new SteamFx();
Fx.X = this.X + Random.NewFloat(this.W);
Fx.Y = this.Y + Random.NewFloat(this.H);
Dungeon.Area.Add(Fx);
}
}
if (this.LeaveVenom > 0) {
Dungeon.Level.Venom(X, Y);
}
} else {
if (!this.IsFlying() && BitHelper.IsBitSet(Info, 0) && !this.HasBuff(Buffs.BURNING)) {
this.AddBuff(new BurningBuff());
}
if (T == Terrain.LAVA && !this.IsFlying() && this.LavaResist == 0) {
this.ModifyHp(-1, null, true);
this.AddBuff(new BurningBuff());
if (this.IsDead()) {
Achievements.Unlock(Achievements.UNLOCK_WINGS);
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
protected override void OnRoomChange() {
base.OnRoomChange();
Bot.Data.Clear();
InGameState.CheckMusic();
if (Dungeon.Depth > -1) {
if (NumCollectedHearts >= 6) {
Achievements.Unlock(Achievements.UNLOCK_MEATBOY);
}
if (HadEnemies && !GotHit) {
Achievements.Unlock(Achievements.UNLOCK_HALO);
}
}
this.ResetHit();
if (this.Room != null) {
if (this.Room is ShopRoom) {
Audio.Play("Shopkeeper");
if (BurningKnight.Instance != null && !BurningKnight.Instance.GetState().Equals("unactive")) {
BurningKnight.Instance.Become("await");
}
}
HadEnemies = false;
}
this.CheckSecrets();
if (Room != null) {
int Count = 0;
foreach (Mob Mob in Mob.All) {
if (Mob.Room == Room) {
Count++;
}
}
if (Count > 0) {
this.Invt = Math.Max(this.Invt, 1f);
}
}
}
public void CheckSecrets() {
if (this.SeeSecrets) {
if (Room != null) {
foreach (Room R in Room.Connected.KeySet()) {
if (R.Hidden) {
for (int Y = R.Top; Y <= R.Bottom; Y++) {
for (int X = R.Left; X <= R.Right; X++) {
if (Dungeon.Level.Get(X, Y) == Terrain.CRACK) {
R.Hidden = false;
BombEntity.Make(R);
Dungeon.Level.Set(X, Y, Terrain.FLOOR_A);
Dungeon.Level.LoadPassable();
Dungeon.Level.AddPhysics();
}
}
}
}
}
}
}
}
public override HpFx ModifyHp(int Amount, Creature From) {
if (Amount > 0 && this.Hp + Amount > 1) {
Tween.To(new Tween.Task(0, 0.4f) {
public override float GetValue() {
return Dungeon.Blood;
}
public override void SetValue(float Value) {
Dungeon.Blood = Value;
}
});
}
return base.ModifyHp(Amount, From);
}
protected override bool IgnoreWater() {
return SlowLiquidResist > 0;
}
protected override void CheckDeath() {
if (this.Hp == 0 && this.NumIronHearts == 0 && this.NumGoldenHearts == 0) {
this.ShouldDie = true;
}
}
public override float RollDamage() {
return GetDamageModifier();
}
public float GetDamageModifier() {
return 1;
}
protected override void DoHurt(int A) {
if (this.NumGoldenHearts > 0) {
int D = Math.Min(this.NumGoldenHearts, -A);
this.NumGoldenHearts -= D;
A += D;
for (int I = 0; I < 10; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Dungeon.Area.Add(Fx);
}
for (int I = 0; I < 10; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Dungeon.Area.Add(Fx);
}
foreach (Mob Mob in Mob.All) {
if (Mob.Room == this.Room) {
Mob.AddBuff(new FrozenBuff().SetDuration(10));
}
}
}
if (this.NumIronHearts > 0) {
int D = Math.Min(this.NumIronHearts, -A);
this.NumIronHearts -= D;
A += D;
}
if (A < 0) {
this.Hp = Math.Max(0, this.Hp + A);
}
}
public override void OnHurt(int A, Entity From) {
base.OnHurt(A, From);
this.GotHit = true;
Dungeon.Flash(Color.WHITE, 0.05f);
Camera.Shake(4f);
Audio.PlaySfx("voice_gobbo_" + Random.NewInt(1, 4), 1f, Random.NewFloat(0.9f, 1.9f));
}
protected override void Die(bool Force) {
if (this.ToDeath) {
return;
}
UiMap.Instance.Hide();
Ui.Ui.OnDeath();
this.Done = false;
int Num = GlobalSave.GetInt("deaths") + 1;
GlobalSave.Put("deaths", Num);
Vector3 Vec = Camera.Game.Project(new Vector3(this.X + this.W / 2, this.Y + this.H / 2, 0));
Vec = Camera.Ui.Unproject(Vec);
Vec.Y = Display.UI_HEIGHT - Vec.Y;
Dungeon.ShockTime = 0;
Dungeon.ShockPos.X = (Vec.X) / Display.UI_WIDTH;
Dungeon.ShockPos.Y = (Vec.Y) / Display.UI_HEIGHT;
this.ToDeath = true;
this.T = 0;
Dungeon.SlowDown(0.5f, 1f);
if (Dungeon.Depth != -3) {
Achievements.Unlock(Achievements.DIE);
}
if (Num >= 50) {
Achievements.Unlock(Achievements.UNLOCK_ISAAC_HEAD);
}
}
public override void Save(FileWriter Writer) {
base.Save(Writer);
this.Inventory.Save(Writer);
Writer.WriteInt32((int) this.Mana);
Writer.WriteInt32(this.ManaMax);
Writer.WriteInt32(this.Level);
Writer.WriteFloat(this.Speed);
Writer.WriteByte((byte) NumIronHearts);
Writer.WriteByte((byte) NumGoldenHearts);
Writer.WriteBoolean(this.GotHit);
Writer.WriteByte((byte) this.Bombs);
Writer.WriteByte((byte) this.Keys);
Writer.WriteInt16((short) this.Money);
Writer.WriteByte((byte) GetTypeId(this.Type));
}
public override void Load(FileReader Reader) {
base.Load(Reader);
this.Inventory.Load(Reader);
Reader.ReadInt32();
this.ManaMax = Reader.ReadInt32();
this.Mana = ManaMax;
this.Level = Reader.ReadInt32();
float Last = this.Speed;
this.Speed = Reader.ReadFloat();
this.NumIronHearts = Reader.ReadByte();
this.NumGoldenHearts = Reader.ReadByte();
this.GotHit = Reader.ReadBoolean();
this.MaxSpeed += (this.Speed - Last) * 7f;
this.SetHat(null);
DoTp(false);
HasBkKey = this.Inventory.Find(BurningKey.GetType());
OnRoomChange();
this.Bombs = Reader.ReadByte();
this.Keys = Reader.ReadByte();
this.Money = Reader.ReadInt16();
Light.SetPosition(this.X + 8, this.Y + 8);
Light.AttachToBody(this.Body, 8, 8, 0);
this.Type = GetTypeFromId(Reader.ReadByte());
}
protected override bool CanHaveBuff(Buff Buff) {
if ((this.Rolling || FireResist > 0) && Buff is BurningBuff) {
return false;
} else if (PoisonResist > 0 && Buff is PoisonedBuff) {
return false;
} else if ((this.Rolling || StunResist > 0) && Buff is FrozenBuff) {
return false;
}
return base.CanHaveBuff(Buff);
}
}
}