using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.npc {
	public class Upgrade : ItemHolder {
		enum Type {
			CONSUMABLE,
			WEAPON,
			ACCESSORY,
			PET,
			PERMANENT,
			NONE,
			DECOR
		}

		public static List<Upgrade> All = new List<>();
		public static bool UpdateEvent;
		public static Upgrade ActiveUpgrade;
		public Type Type = Type.PERMANENT;
		private Item Item;
		private string Str;
		protected float Z;
		private Body Body;
		private int Price = 0;
		private string CostStr = "";
		private int CostW = 0;
		private int NameW = 0;
		public string Idd = "";
		private bool Hidden;
		private bool Checked;
		private float Al;
		private bool Colliding;

		public static int GetTypeId(Type Type) {
			switch (Type) {
				case CONSUMABLE: {
					return 0;
				}

				case WEAPON: {
					return 1;
				}

				case ACCESSORY: {
					return 2;
				}

				case PET: {
					return 3;
				}

				case PERMANENT: {
					return 4;
				}

				case NONE: 
				default:{
					return 5;
				}

				case DECOR: {
					return 6;
				}
			}
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Type = Type.Values()[Reader.ReadByte()];

			try {
				this.Idd = Reader.ReadString();
				this.Str = Reader.ReadString();

				if (Str != null) {
					ItemRegistry.Pair Pair = ItemRegistry.Items.Get(this.Str);
					Pair.Busy = true;
					this.Item = (Item) Pair.Type.NewInstance();
					this.Price = Pair.Cost;
					this.SetupInfo();
					this.CreateBody();
				} 
			} catch (InstantiationException) {
				E.PrintStackTrace();
			} catch (IllegalAccessException) {
				E.PrintStackTrace();
			}
		}

		public override Void Init() {
			base.Init();
			this.T = Random.NewFloat(10f);
			All.Add(this);
		}

		protected Item GenerateItem() {
			foreach (ItemRegistry.Pair Pair in ItemRegistry.Items.Values()) {
				if (!Pair.Busy && Pair.Pool == this.Type && !Pair.Unlocked(Pair.Id)) {
					this.Str = Pair.Id;
					Pair.Busy = true;

					try {
						this.Price = Pair.Cost;
						this.SetupInfo();

						return (Item) Pair.Type.NewInstance();
					} catch (InstantiationException) {
						E.PrintStackTrace();
					} catch (IllegalAccessException) {
						E.PrintStackTrace();
					}
				} 
			}

			return null;
		}

		private Void SetupInfo() {
			if (this.Item == null) {
				this.CostStr = "";

				return;
			} 

			this.CostStr = "" + this.Price;
			Graphics.Layout.SetText(Graphics.Small, this.CostStr);
			this.CostW = (int) Graphics.Layout.Width;
			Graphics.Layout.SetText(Graphics.Medium, this.Item.GetName());
			this.NameW = (int) Graphics.Layout.Width;
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			this.CheckItem();
			Writer.WriteByte((byte) GetTypeId(Type));
			Writer.WriteString(this.Idd);
			Writer.WriteString(this.Str);
		}

		public override Void Update(float Dt) {
			if (!Checked) {
				Checked = true;

				foreach (Trader Trader in Trader.All) {
					if (Trader.Id.Equals(this.Idd) && Trader.Saved) {
						this.Hidden = false;

						return;
					} 
				}

				this.Hidden = true;
			} 

			if (this.Hidden) {
				return;
			} 

			if (Colliding && ActiveUpgrade == null && Npc.Active == null) {
				ActiveUpgrade = this;
			} else if (!Colliding && ActiveUpgrade == this) {
				ActiveUpgrade = null;
			} 

			this.CheckItem();
			this.Al = MathUtils.Clamp(0f, 1f, this.Al + ((ActiveUpgrade == this ? 1 : 0) - this.Al) * Dt * 10f);
			this.Z += (Math.Cos((this.T * 2.4f)) / 10f * Dt * 60.0);
			this.Z = MathUtils.Clamp(0f, 5f, this.Z);

			if (this.Body != null) {
				this.Body.SetTransform(this.X + 8 - this.W / 2, this.Y + this.Z + 8 - this.H / 2, 0f);
			} 

			if (ActiveUpgrade == this && Input.Instance.WasPressed("interact")) {
				int Count = GlobalSave.GetInt("num_coins");

				if (Count < this.Price) {
					Audio.PlaySfx("item_nocash");
					Camera.Shake(3f);
				} else {
					Count -= this.Price;
					GlobalSave.Put("num_coins", Count);
					this.PlaySfx("item_purchase");
					UpdateEvent = true;
					Achievements.Unlock("SHOP_" + this.Str.ToUpperCase());

					for (int I = 0; I < 9; I++) {
						PoofFx Fx = new PoofFx();
						Fx.X = this.X + this.W / 2;
						Fx.Y = this.Y + this.H / 2;
						Dungeon.Area.Add(Fx);
					}

					for (int I = 0; I < 14; I++) {
						Confetti C = new Confetti();
						C.X = this.X + Random.NewFloat(this.W);
						C.Y = this.Y + Random.NewFloat(this.H);
						C.Vel.X = Random.NewFloat(-30f, 30f);
						C.Vel.Y = Random.NewFloat(30f, 40f);
						Dungeon.Area.Add(C);
					}

					this.Item = null;
					this.Str = null;
					this.Body = World.RemoveBody(this.Body);
				}

			} 

			base.Update(Dt);
		}

		private Void CheckItem() {
			if (this.Item == null) {
				this.Item = this.GenerateItem();

				if (this.Item == null) {
					this.Hidden = true;
				} else {
					this.SetupInfo();
					this.CreateBody();
				}

			} 
		}

		private Void CreateBody() {
			this.W = this.Item.GetSprite().GetRegionWidth();
			this.H = this.Item.GetSprite().GetRegionHeight();
			this.Body = World.RemoveBody(this.Body);
			this.Body = World.CreateSimpleBody(this, 0f, 0f, this.W, this.H, BodyDef.BodyType.DynamicBody, true);
			this.Body.SetTransform(this.X, this.Y, 0f);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
			All.Remove(this);
		}

		public override Void RenderShadow() {
			if (!this.Hidden) {
				base.RenderShadow();
			} 
		}

		public Void RenderSigns() {
			if (Hidden || Item == null) {
				return;
			} 

			if (this.Al > 0.05f && !Ui.HideUi) {
				Graphics.Medium.SetColor(1f, 1f, 1f, this.Al);
				Graphics.Print(this.Item.GetName(), Graphics.Medium, this.X + 8 - NameW / 2, this.Y + this.H + 8);
				Graphics.Medium.SetColor(1f, 1f, 1f, 1f);
			} 
		}

		public override Void Render() {
			if (Hidden || Item == null) {
				return;
			} 

			TextureRegion Sprite = this.Item.GetSprite();
			float A = (float) (Math.Cos((this.T * 3f)) * 8f);
			float Sy = (float) (1f + Math.Sin((this.T * 2f)) / 10f);
			Graphics.Batch.End();

			if (this.Al > 0.05f && !Ui.HideUi) {
				Mob.Shader.Begin();
				Mob.Shader.SetUniformf("u_color", new Vector3(1f, 1f, 1f));
				Mob.Shader.SetUniformf("u_a", this.Al);
				Mob.Shader.End();
				Graphics.Batch.SetShader(Mob.Shader);
				Graphics.Batch.Begin();

				for (int Xx = -1; Xx < 2; Xx++) {
					for (int Yy = -1; Yy < 2; Yy++) {
						if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
							Graphics.Render(Sprite, this.X + 8 + Xx, this.Y + Z + 8 + Yy, A, W / 2, H / 2, false, false, 1f, Sy);
						} 
					}
				}

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
			} 

			Graphics.Batch.Begin();
			Graphics.Print(this.CostStr, Graphics.Small, this.X + 8 - this.CostW / 2, this.Y - 8);
			Graphics.Batch.End();
			WeaponBase.Shader.Begin();
			WeaponBase.Shader.SetUniformf("a", 1f);
			WeaponBase.Shader.SetUniformf("gray", 1f);
			WeaponBase.Shader.SetUniformf("time", Dungeon.Time + this.T);
			WeaponBase.Shader.End();
			Graphics.Batch.SetShader(WeaponBase.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, this.X + 8, this.Y + Z + 8, A, W / 2, H / 2, false, false, 1f, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) {
				Colliding = true;
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) {
				Colliding = false;
			} 
		}
	}
}
