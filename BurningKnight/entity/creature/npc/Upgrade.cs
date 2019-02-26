using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.entity.level.save;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.npc {
	public class Upgrade : ItemHolder {
		public static List<Upgrade> All = new List<>();
		public static bool UpdateEvent;
		public static Upgrade ActiveUpgrade;
		private float Al;
		private Body Body;
		private bool Checked;
		private bool Colliding;
		private string CostStr = "";
		private int CostW;
		private bool Hidden;
		public string Idd = "";
		private Item Item;
		private int NameW;
		private int Price;
		private string Str;
		public Type Type = Type.PERMANENT;
		protected float Z;

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
				default: {
					return 5;
				}

				case DECOR: {
					return 6;
				}
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Type = Type.Values()[Reader.ReadByte()];

			try {
				Idd = Reader.ReadString();
				Str = Reader.ReadString();

				if (Str != null) {
					ItemRegistry.Pair Pair = ItemRegistry.Items.Get(Str);
					Pair.Busy = true;
					Item = (Item) Pair.Type.NewInstance();
					Price = Pair.Cost;
					SetupInfo();
					CreateBody();
				}
			}
			catch (InstantiationException) {
				E.PrintStackTrace();
			}
			catch (IllegalAccessException) {
				E.PrintStackTrace();
			}
		}

		public override void Init() {
			base.Init();
			T = Random.NewFloat(10f);
			All.Add(this);
		}

		protected Item GenerateItem() {
			foreach (ItemRegistry.Pair Pair in ItemRegistry.Items.Values())
				if (!Pair.Busy && Pair.Pool == this.Type && !Pair.Unlocked(Pair.Id)) {
					Str = Pair.Id;
					Pair.Busy = true;

					try {
						Price = Pair.Cost;
						SetupInfo();

						return (Item) Pair.Type.NewInstance();
					}
					catch (InstantiationException) {
						E.PrintStackTrace();
					}
					catch (IllegalAccessException) {
						E.PrintStackTrace();
					}
				}

			return null;
		}

		private void SetupInfo() {
			if (Item == null) {
				CostStr = "";

				return;
			}

			CostStr = "" + Price;
			Graphics.Layout.SetText(Graphics.Small, CostStr);
			CostW = (int) Graphics.Layout.Width;
			Graphics.Layout.SetText(Graphics.Medium, Item.GetName());
			NameW = (int) Graphics.Layout.Width;
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			CheckItem();
			Writer.WriteByte((byte) GetTypeId(Type));
			Writer.WriteString(Idd);
			Writer.WriteString(Str);
		}

		public override void Update(float Dt) {
			if (!Checked) {
				Checked = true;

				foreach (Trader Trader in Trader.All)
					if (Trader.Id.Equals(Idd) && Trader.Saved) {
						Hidden = false;

						return;
					}

				Hidden = true;
			}

			if (Hidden) return;

			if (Colliding && ActiveUpgrade == null && Npc.Active == null)
				ActiveUpgrade = this;
			else if (!Colliding && ActiveUpgrade == this) ActiveUpgrade = null;

			CheckItem();
			Al = MathUtils.Clamp(0f, 1f, Al + ((ActiveUpgrade == this ? 1 : 0) - Al) * Dt * 10f);
			Z += Math.Cos(T * 2.4f) / 10f * Dt * 60.0;
			Z = MathUtils.Clamp(0f, 5f, Z);

			if (Body != null) Body.SetTransform(this.X + 8 - W / 2, this.Y + Z + 8 - H / 2, 0f);

			if (ActiveUpgrade == this && Input.Instance.WasPressed("interact")) {
				var Count = GlobalSave.GetInt("num_coins");

				if (Count < Price) {
					Audio.PlaySfx("item_nocash");
					Camera.Shake(3f);
				}
				else {
					Count -= Price;
					GlobalSave.Put("num_coins", Count);
					PlaySfx("item_purchase");
					UpdateEvent = true;
					Achievements.Unlock("SHOP_" + Str.ToUpperCase());

					for (var I = 0; I < 9; I++) {
						var Fx = new PoofFx();
						Fx.X = this.X + W / 2;
						Fx.Y = this.Y + H / 2;
						Dungeon.Area.Add(Fx);
					}

					for (var I = 0; I < 14; I++) {
						var C = new Confetti();
						C.X = this.X + Random.NewFloat(W);
						C.Y = this.Y + Random.NewFloat(H);
						C.Vel.X = Random.NewFloat(-30f, 30f);
						C.Vel.Y = Random.NewFloat(30f, 40f);
						Dungeon.Area.Add(C);
					}

					Item = null;
					Str = null;
					Body = World.RemoveBody(Body);
				}
			}

			base.Update(Dt);
		}

		private void CheckItem() {
			if (Item == null) {
				Item = GenerateItem();

				if (Item == null) {
					Hidden = true;
				}
				else {
					SetupInfo();
					CreateBody();
				}
			}
		}

		private void CreateBody() {
			W = Item.GetSprite().GetRegionWidth();
			H = Item.GetSprite().GetRegionHeight();
			Body = World.RemoveBody(Body);
			Body = World.CreateSimpleBody(this, 0f, 0f, W, H, BodyDef.BodyType.DynamicBody, true);
			Body.SetTransform(this.X, this.Y, 0f);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
			All.Remove(this);
		}

		public override void RenderShadow() {
			if (!Hidden) base.RenderShadow();
		}

		public void RenderSigns() {
			if (Hidden || Item == null) return;

			if (Al > 0.05f && !Ui.HideUi) {
				Graphics.Medium.SetColor(1f, 1f, 1f, Al);
				Graphics.Print(Item.GetName(), Graphics.Medium, this.X + 8 - NameW / 2, this.Y + H + 8);
				Graphics.Medium.SetColor(1f, 1f, 1f, 1f);
			}
		}

		public override void Render() {
			if (Hidden || Item == null) return;

			TextureRegion Sprite = Item.GetSprite();
			var A = Math.Cos(T * 3f) * 8f;
			var Sy = 1f + Math.Sin(T * 2f) / 10f;
			Graphics.Batch.End();

			if (Al > 0.05f && !Ui.HideUi) {
				Mob.Shader.Begin();
				Mob.Shader.SetUniformf("u_color", new Vector3(1f, 1f, 1f));
				Mob.Shader.SetUniformf("u_a", Al);
				Mob.Shader.End();
				Graphics.Batch.SetShader(Mob.Shader);
				Graphics.Batch.Begin();

				for (var Xx = -1; Xx < 2; Xx++)
				for (var Yy = -1; Yy < 2; Yy++)
					if (Math.Abs(Xx) + Math.Abs(Yy) == 1)
						Graphics.Render(Sprite, this.X + 8 + Xx, this.Y + Z + 8 + Yy, A, W / 2, H / 2, false, false, 1f, Sy);

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
			}

			Graphics.Batch.Begin();
			Graphics.Print(CostStr, Graphics.Small, this.X + 8 - CostW / 2, this.Y - 8);
			Graphics.Batch.End();
			WeaponBase.Shader.Begin();
			WeaponBase.Shader.SetUniformf("a", 1f);
			WeaponBase.Shader.SetUniformf("gray", 1f);
			WeaponBase.Shader.SetUniformf("time", Dungeon.Time + T);
			WeaponBase.Shader.End();
			Graphics.Batch.SetShader(WeaponBase.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, this.X + 8, this.Y + Z + 8, A, W / 2, H / 2, false, false, 1f, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) Colliding = true;
		}

		public override void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) Colliding = false;
		}

		private enum Type {
			CONSUMABLE,
			WEAPON,
			ACCESSORY,
			PET,
			PERMANENT,
			NONE,
			DECOR
		}
	}
}