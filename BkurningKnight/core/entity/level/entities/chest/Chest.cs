using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.entity.pool;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.chest {
	public class Chest : SaveableEntity {
		protected void _Init() {
			{
				H = 13;
			}
		}

		private AnimationData Data;
		protected Body Body;
		protected Body Sensor;
		protected bool Open;
		protected Item Item;
		protected bool Create;
		public static List<Chest> All = new List<>();
		public bool Weapon;
		public bool Locked = true;
		public bool Burning;
		public static TextureRegion KeyRegion = Graphics.GetTexture("item-key_c");
		public static Animation LockAnimations = Animation.Make("door-lock", "-gold");
		private AnimationData Unlock = LockAnimations.Get("open");
		private AnimationData LockUnlock = Door.GoldLockAnimation.Get("open");
		private bool RenderUnlock;
		public static TextureRegion IdleLock = LockAnimations.GetFrames("idle").Get(0).Frame;
		private TextureRegion HalfBroken = GetAnim().GetFrames("break").Get(0).Frame;
		private TextureRegion Broken = GetAnim().GetFrames("break").Get(1).Frame;
		private byte Hp = 14;
		private float Last;
		private bool Collided;
		private bool CreateLoot;
		private float Al;
		private bool Colliding;
		private float Damage;
		private float LastFlame;
		private bool DrawOpenAnim;
		private float Vt;

		public override Void Init() {
			base.Init();
			this.Data = this.GetClosedAnim();
			this.Data.SetAutoPause(true);
			this.Body = World.CreateSimpleBody(this, 4, 8, 8, 1, BodyDef.BodyType.DynamicBody, false);
			this.Sensor = World.CreateSimpleBody(this, 0, 0, 16, 12, BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.Sensor.SetTransform(this.X, this.Y, 0);
			MassData Data = new MassData();
			Data.Mass = 1000000000000000f;
			this.Body.SetMassData(Data);
			All.Add(this);
		}

		public Item Generate() {
			return null;
		}

		public Void SetItem(Item Item) {
			this.Item = Item;
			this.Item.Generate();
		}

		public static Pool MakePool<Item> (ItemRegistry.Quality Quality, bool Weapon, bool Any) {
			Pool<Item> Pool = new Pool<>();

			foreach (Map.Entry<string, ItemRegistry.Pair> Both in ItemRegistry.Items.EntrySet()) {
				ItemRegistry.Pair Item = Both.GetValue();

				if (ItemRegistry.Check(Item.Quality, Quality) && (Any || (Weapon == WeaponBase.GetType().IsAssignableFrom(Item.Type))) && Item.Unlocked(Both.GetKey()) && (Player.Instance == null || Player.Instance.GetInventory().FindItem(Item.Type) == null)) {
					if (IsVeganFine(Item.Type)) {
						Pool.Add(Item.Type, Item.Chance * (Item.Warrior * Player.GetStaticWarrior() + Item.Mage * Player.GetStaticMage() + Item.Ranged * Player.GetStaticRanger()));
					} 
				} 
			}

			return Pool;
		}

		public static Item Generate(ItemRegistry.Quality Quality, bool Weapon) {
			Pool<Item> Pool = MakePool(Quality, Weapon, false);
			Item Item = Pool.Generate();

			if (Item == null) {
				Log.Error("Failed to generate item " + Quality + " (weapon = " + Weapon + ")");

				return new KeyB();
			} 

			return Item;
		}

		public override Void OnCollision(Entity Entity) {
			if (this.Hp == 0) {
				return;
			} 

			if (Entity is Creature) {
				if (((Creature) Entity).HasBuff(Buffs.BURNING)) {
					this.Burning = true;
				} else if (this.Burning) {
					((Creature) Entity).AddBuff(new BurningBuff());
				} 
			} 

			if (Entity is Player) {
				if (this.Locked) {
					this.Colliding = true;
				} else if (!this.Open) {
					this.Locked = false;
					this.Open = true;
					this.PlaySfx("chest");
					this.Data = this.GetOpenAnim();
					this.Data.SetListener(new AnimationData.Listener() {
						public override Void OnEnd() {
							Create = true;
						}
					});
				} 
			} 

			if ((Entity is Projectile || Entity is Weapon)) {
				if (Dungeon.Depth == -3) {
					return;
				} 

				if (Entity is Projectile) {
					if (!(((Projectile) Entity).Owner is Player)) {
						return;
					} 

					((Projectile) Entity).Remove();
				} else if (Entity is Weapon) {
					if (!(((Weapon) Entity).GetOwner() is Player)) {
						return;
					} 
				} 

				Hit();
			} 
		}

		public Void Explode() {
			if (this.Hp == 0) {
				return;
			} 

			this.Hp = 0;
			this.Burning = false;

			for (int I = 0; I < 10; I++) {
				PoofFx Fx = new PoofFx();
				Fx.X = this.X + this.W / 2;
				Fx.Y = this.Y + this.H / 2;
				Dungeon.Area.Add(Fx);
			}

			this.Locked = false;
			this.CreateLoot = true;
			this.Body = World.RemoveBody(this.Body);
			this.Sensor = World.RemoveBody(this.Sensor);
		}

		private Void Hit() {
			if (this.Hp == 0) {
				return;
			} 

			this.Hp -= 1;

			if (this.Hp <= 0) {
				this.Hp = 0;
				this.Burning = false;

				for (int I = 0; I < 10; I++) {
					PoofFx Fx = new PoofFx();
					Fx.X = this.X + this.W / 2;
					Fx.Y = this.Y + this.H / 2;
					Dungeon.Area.Add(Fx);
				}

				this.Locked = false;
				this.CreateLoot = true;
				this.Body = World.RemoveBody(this.Body);
				this.Sensor = World.RemoveBody(this.Sensor);
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) {
				this.Colliding = false;
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
			this.Sensor = World.RemoveBody(this.Sensor);
			All.Remove(this);
		}

		public Void ToMimic() {
			Mimic Chest = new Mimic();
			Chest.X = this.X;
			Chest.Y = this.Y;
			Chest.Weapon = this.Weapon;
			Chest.Locked = this.Locked;
			Dungeon.Area.Add(Chest);
			LevelSave.Add(Chest);
			this.Done = true;
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Open = Reader.ReadBoolean();
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);

			if (!this.Open) {
				string Name = Reader.ReadString();

				try {
					Class Clazz = Class.ForName(Name);
					Constructor Constructor = Clazz.GetConstructor();
					Object Object = Constructor.NewInstance();
					Item Item = (Item) Object;
					Item.Load(Reader);
					this.Item = Item;
				} catch (Exception) {
					Log.Error(Name);
					Dungeon.ReportException(E);
				}
			} 

			if (this.Item == null && !this.Open) {
				Log.Error("Something wrong with chest");
			} 

			if (this.Open) {
				this.Data = this.GetOpenedAnim();
			} 

			this.Locked = Reader.ReadBoolean();
			this.Hp = Reader.ReadByte();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Open);

			if (!this.Open && this.Item == null) {
				Log.Error("Something wrong when saving");
			} 

			if (!this.Open) {
				Writer.WriteString(this.Item.GetClass().GetName());
				this.Item.Save(Writer);
			} 

			Writer.WriteBoolean(this.Locked);
			Writer.WriteByte(this.Hp);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (RenderUnlock) {
				if (LockUnlock.Update(Dt)) {
					RenderUnlock = false;
				} 
			} 

			if (!this.Open) {
				this.Last += Dt;

				if (Locked) {
					this.Vt = Math.Max(0, Vt - Dt);
				} 

				if (this.Last >= 0.6f) {
					Last = 0;
					Spark.RandomOn(this);
				} 
			} 

			if (this.Hp == 0) {
				this.Burning = false;
			} 

			if (CreateLoot) {
				if (Dungeon.Depth != -3) {
					if (Random.Chance(50)) {
						HeartFx Fx = new HeartFx();
						Fx.X = this.X + (this.W - Fx.W) / 2;
						Fx.Y = this.Y + (this.H - Fx.H) / 2;
						Dungeon.Area.Add(Fx.Add());
						Fx.RandomVelocity();
					} 

					if (Random.Chance(30)) {
						ItemHolder Fx = new ItemHolder(new KeyC());
						Fx.X = this.X + (this.W - Fx.W) / 2;
						Fx.Y = this.Y + (this.H - Fx.H) / 2;
						Dungeon.Area.Add(Fx.Add());
					} 

					if (Random.Chance(30)) {
						ItemHolder Fx = new ItemHolder(new Gold());
						Fx.GetItem().Generate();
						Fx.X = this.X + (this.W - Fx.W) / 2;
						Fx.Y = this.Y + (this.H - Fx.H) / 2;
						Dungeon.Area.Add(Fx.Add());
					} 
				} 

				CreateLoot = false;
			} 

			this.T += Dt;

			if (this.Item != null && this.Create) {
				this.Open();
			} 

			if (this.Data != null && this.Data.Update(Dt)) {
				if (this.Data == this.GetOpenAnim()) {
					this.Data = this.GetOpenedAnim();
				} 
			} 

			if (this.Body != null) {
				this.Sensor.SetTransform(this.X, this.Y, 0);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 

			this.Al += ((this.Colliding && !DrawOpenAnim ? 1f : 0f) - this.Al) * Dt * 8;

			if (this.Al >= 0.5f && Input.Instance.WasPressed("interact")) {
				if (this.Locked) {
					{
						Item Key = Player.Instance.GetInventory().FindItem(KeyC.GetType());

						if (Key == null && (Player.Instance.GetKeys() == 0)) {
							this.Colliding = true;
							Vt = 1;
							Player.Instance.PlaySfx("item_nocash");
							Camera.Shake(6);

							return;
						} 

						Player.Instance.PlaySfx("unlock");
						this.PlaySfx("chest");
						DrawOpenAnim = true;
						RenderUnlock = true;

						if (Key != null) {
							Key.SetCount(Key.GetCount() - 1);
						} else {
							Player.Instance.SetKeys(Player.Instance.GetKeys() - 1);
						}


						this.Locked = false;
						this.Open = true;
						this.Data = this.GetOpenAnim();
						this.Data.SetListener(new AnimationData.Listener() {
							public override Void OnEnd() {
								Create = true;
							}
						});
					}
				} 
			} 

			if (this.DrawOpenAnim) {
				if (Unlock.Update(Dt)) {
					this.DrawOpenAnim = false;
				} 
			} 

			if (this.Burning) {
				LastFlame += Dt;
				InGameState.Burning = true;

				if (this.LastFlame >= 0.05f) {
					this.LastFlame = 0;
					TerrainFlameFx Fx = new TerrainFlameFx();
					Fx.X = this.X + Random.NewFloat(this.W);
					Fx.Y = this.Y + Random.NewFloat(this.H) - 4;
					Fx.Depth = 1;
					Dungeon.Area.Add(Fx);
				} 

				Damage += Dt * 2;

				if (Damage >= 1f) {
					Damage = 0f;
					Hit();
				} 
			} else {
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

		}

		public static bool IsVeganFine(Class Item) {
			if (!Settings.Vegan) {
				return true;
			} 

			if (Item == KillerItem.GetType() && GameSave.RunId < 10) {
				return false;
			} 

			return true;
		}

		public Void Open() {
			ItemHolder Holder = new ItemHolder(this.Item);
			Holder.X = this.X + (this.W - this.Item.GetSprite().GetRegionWidth()) / 2;
			Holder.Y = this.Y - 3;
			Dungeon.Area.Add(Holder);
			LevelSave.Add(Holder);
			this.Item = null;
			this.Create = false;
			this.Data = this.GetOpenedAnim();
			this.Open = true;

			for (int I = 0; I < 15; I++) {
				Confetti C = new Confetti();
				C.X = this.X + Random.NewFloat(this.W);
				C.Y = this.Y + Random.NewFloat(this.H);
				C.Vel.X = Random.NewFloat(-30f, 30f);
				C.Vel.Y = Random.NewFloat(30f, 40f);
				Dungeon.Area.Add(C);
			}
		}

		public static Chest Random() {
			float R = Random.NewFloat();

			if (R < 0.5f) {
				WoodenChest Chest = new WoodenChest();

				if (Random.Chance(30)) {
					Chest.Locked = false;
				} 

				return Chest;
			} else if (R < 0.8f) {
				return new IronChest();
			} 

			return new GoldenChest();
		}

		public override Void Render() {
			if (this.Data != null) {
				TextureRegion Sprite;

				if (this.Hp != 0 && (this.Open || this.Hp > 6)) {
					Sprite = this.Data.GetCurrent().Frame;
				} else if (this.Hp == 0) {
					Sprite = Broken;
				} else {
					Sprite = HalfBroken;
				}


				int W = Sprite.GetRegionWidth();
				float Sx = 1f;
				float Sy = 1f;
				Graphics.Render(Sprite, this.X + W / 2 - 1, this.Y, 0, W / 2, 0, false, false, Sx, Sy);
			} 

			float X = this.X + (W - IdleLock.GetRegionWidth()) / 2;
			float Y = this.Y + (H - IdleLock.GetRegionHeight()) / 2 + (float) Math.Sin(this.T) * 1.8f;

			if (this.Locked) {
				if (Al > 0) {
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
								Graphics.Render(this.IdleLock, X + Xx, Y + Yy);
							} 
						}
					}

					Graphics.Batch.End();
					Graphics.Batch.SetShader(null);
					Graphics.Batch.Begin();
				} 

				Graphics.Render(this.IdleLock, X, Y);
			} else if (DrawOpenAnim) {
				Unlock.Render(X, Y, false);
			} 

			if ((this.Locked || DrawOpenAnim) && Al > 0) {
				float V = Vt <= 0 ? 0 : (float) (Math.Cos(Dungeon.Time * 18f) * 5 * (Vt));
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(KeyRegion, this.X + (16 - KeyRegion.GetRegionWidth()) / 2 + V, this.Y + 12);
				Graphics.Batch.SetColor(1, 1, 1, 1);
			} 

			if (RenderUnlock) {
				LockUnlock.Render(X, Y, false);
			} 
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H);
		}

		protected AnimationData GetClosedAnim() {
			return null;
		}

		protected Animation GetAnim() {
			return null;
		}

		protected AnimationData GetOpenedAnim() {
			return null;
		}

		protected AnimationData GetOpenAnim() {
			return null;
		}

		public Chest() {
			_Init();
		}
	}
}
