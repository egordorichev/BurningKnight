using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.shop;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class ItemHolder : SaveableEntity {
		protected Body Body;
		protected float StartX = 0f;
		protected float StartY = 0f;
		protected bool Fake = false;
		protected Item Item;
		public bool CreateBody;
		public bool Falling;
		public bool Auto;
		protected float Last;
		public ItemPrice Price;
		private float Z;
		private int Hw;
		private int Hh;
		private bool Added;
		public float Al;
		private float Sz = 1f;
		private float Fall = 1f;
		public static List<ItemHolder> All = new List<>();

		public ItemHolder(Item Item) {
			this.SetItem(Item);
		}

		public ItemHolder() {

		}

		public Void SetItem(Item Item) {
			this.Item = Item;
			this.Body = World.RemoveBody(this.Body);

			if (Item == null) {
				return;
			} 

			if (this.Item.GetSprite() == null) {
				return;
			} 

			CreateBody = true;
			this.W = Item.GetSprite().GetRegionWidth();
			this.H = Item.GetSprite().GetRegionHeight();
		}

		protected Body CreateSimpleBody(int X, int Y, int W, int H, BodyDef.BodyType Type, bool Sensor) {
			this.Hw = W;
			this.Hh = H;

			return World.CreateSimpleBody(this, X, Y, W, H, Type, Sensor);
		}

		public Void RandomVelocity() {
			float A = Random.NewFloat((float) (Math.PI * 2));
			float F = Random.NewFloat(60f, 150f);
			this.Velocity.X = (float) (Math.Cos(A) * F);
			this.Velocity.Y = (float) (Math.Sin(A) * F);
		}

		public Void VelocityToMouse() {
			float Dx = Input.Instance.WorldMouse.X - this.X;
			float Dy = Input.Instance.WorldMouse.Y - this.Y;
			float A = (float) Math.Atan2(Dy, Dx);
			this.Velocity.X = (float) (Math.Cos(A) * 100f);
			this.Velocity.Y = (float) (Math.Sin(A) * 100f);
		}

		public Void Sale() {
			Item.Sale = true;
			Item.Price = (int) Math.Max(0.0, Math.Floor(Item.Price / 2));
			Added = false;

			if (Price != null) {
				Price.Done = true;
			} 
		}

		public bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Chest) {
				return false;
			} 

			if (Item is Gold && !(Entity is Player)) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override Void Update(float Dt) {
			if (Falling) {
				Fall -= Dt;

				if (Fall <= 0) {
					Done = true;
				} 
			} 

			if (CreateBody) {
				if (!Fake) {
					this.Body = this.CreateSimpleBody(-2, -2, Item.GetSprite().GetRegionWidth() + 4, Item.GetSprite().GetRegionHeight() + 4, BodyDef.BodyType.DynamicBody, false);
					this.Body.SetTransform(this.X, this.Y + this.Z, 0f);
				} 

				CreateBody = false;
			} 

			if (this.Item == null) {
				return;
			} 

			if (this.Item.Shop && !Added) {
				Added = true;
				ItemPrice Price = new ItemPrice();
				this.Item.Price = this.Item.GetPrice();
				this.Item.Price = Math.Max(1, this.Item.Price + Random.NewInt(-3, 3));
				Price.X = this.X + this.W / 2;
				Price.Y = this.Y - 6f - (16 - this.H) / 2;
				Price.Price = this.Item.Price;
				Price.Sale = this.Item.Sale;
				this.Price = Price;
				Dungeon.Area.Add(Price);
			} 

			if (!this.Item.Shop && !Falling) {
				bool Found = false;
				int X = (int) (Math.Floor(((this.X) / 16)) - 1);

				while (X < Math.Ceil(((this.X + this.Hw + 8) / 16))) {
					int Y = (int) (Math.Floor(((this.Y) / 16)) - 1);

					while (Y < Math.Ceil(((this.Y + 16f + this.Hh) / 16))) {
						if (X < 0 || Y < 0 || X >= Level.GetWidth() || Y >= Level.GetHeight()) {
							Y++;

							continue;
						} 

						if (CollisionHelper.Check(this.X, this.Y, W, H, X * 16f, Y * 16f - 8f, 32f, 32f)) {
							int I = Level.ToIndex(X, Y);
							int L = Dungeon.Level.Data[I];

							if (L < 1 || L == Terrain.FLOOR_A || L == Terrain.FLOOR_B || L == Terrain.FLOOR_C || L == Terrain.FLOOR_D) {
								Found = true;

								break;
							} 
						} 

						if (Found) {
							break;
						} 

						Y++;
					}

					if (Found) {
						break;
					} 

					X++;
				}

				if (!Found) {
					Log.Error("Fallling");
					Falling = true;
				} 
			} 

			this.T += Dt;
			this.Last += Dt;

			if (this.Done) {
				return;
			} 

			if (this.Last > 0.5f) {
				this.Last = 0f;
				Spark.RandomOn(this.X, this.Y, this.Hw, this.Hh);
			} 

			base.Update(Dt);

			if (this.Body != null) {
				if (this.Item.Shop) {
					World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0f);
				} else {
					this.X = this.Body.GetPosition().X;
					this.Y = this.Body.GetPosition().Y - this.Z;
				}

			} 

			this.Velocity.Mul(0.9f);

			if (!InGameState.Dark && Item is Gold && Item.AutoPickup && !Done) {
				Room Room = Dungeon.Level.FindRoomFor(this.X + this.W / 2, this.Y + this.H / 2);

				if (Room != null && !(Room is ShopRoom) && Room == Player.Instance.Room && !Room.Hidden) {
					float Dx = Player.Instance.X + Player.Instance.W / 2 - this.X - this.W / 2;
					float Dy = Player.Instance.Y + Player.Instance.H / 2 - this.Y - this.H / 2;
					float D = (float) Math.Sqrt((Dx * Dx + Dy * Dy));
					float F = 20f;
					this.Velocity.X += (Dx / D) * F;
					this.Velocity.Y += (Dy / D) * F;
				} 
			} 

			this.Sz = Math.Max(1f, this.Sz - this.Sz * Dt);

			if (this.Velocity.Len() <= 0.1f) {
				this.Velocity.Mul(0f);
				this.X = Math.Round(this.X);
				this.Y = Math.Round(this.Y);
				this.Z += (Math.Cos((this.T * 1.7f)) / 5f * (this.Sz / 2) * Dt * 60.0);
				this.Z = MathUtils.Clamp(0f, 5f, this.Z);

				if (this.Body != null) {
					World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0f);
				} 
			} 

			this.Item.Update(Dt);

			if (this.Body != null) {
				this.Body.SetLinearVelocity(this.Velocity);
			} 
		}

		public override Void Init() {
			base.Init();
			StartX = X;
			StartY = Y;
			this.T = Random.NewFloat(32f);
			this.Last = Random.NewFloat(1f);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0f);
			} 

			All.Add(this);
		}

		public override Void Destroy() {
			base.Destroy();

			if (Price != null) {
				Price.Remove();
				Price = null;
			} 

			this.Body = World.RemoveBody(this.Body);
			All.Remove(this);
		}

		public override Void Render() {
			if (this.Item == null) {
				return;
			} 

			TextureRegion Sprite = this.Item.GetSprite();
			float A = (float) (Math.Cos((this.T * 3f)) * 8f * Sz);
			float Sy = (float) ((1f + Math.Sin((this.T * 2f)) / 10f) * Fall);
			Graphics.Batch.End();
			float Dt = Gdx.Graphics.GetDeltaTime();
			this.Al = MathUtils.Clamp(0f, 1f, this.Al + (((Player.Instance.PickupFx != null && Player.Instance.PickupFx.Item == this) ? 1 : 0) - this.Al) * Dt * 10f);

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
							Graphics.Render(Sprite, this.X + W / 2 + Xx, this.Y + this.Z + H / 2 + Yy, A, W / 2, H / 2, false, false, Fall, Sy);
						} 
					}
				}

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
			} 

			WeaponBase.Shader.Begin();
			WeaponBase.Shader.SetUniformf("a", 1f);
			WeaponBase.Shader.SetUniformf("gray", 1f);
			WeaponBase.Shader.SetUniformf("time", Dungeon.Time + this.T);
			WeaponBase.Shader.End();
			Graphics.Batch.SetShader(WeaponBase.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, this.X + W / 2, this.Y + this.Z + H / 2, A, W / 2, H / 2, false, false, Fall, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W * Fall, this.H * Fall, this.Z);
		}

		public Item GetItem() {
			return Item;
		}

		public Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature) {
				Tween.To(new Tween.Task(4f, 0.3f) {
					public override float GetValue() {
						return Sz;
					}

					public override Void SetValue(float Value) {
						Sz = Value;
					}
				});
			} 
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Item != null);

			if (this.Item != null) {
				Writer.WriteString(this.Item.GetClass().GetName());
				this.Item.Save(Writer);
			} 
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);

			if (Reader.ReadBoolean()) {
				string Type = Reader.ReadString();

				try {
					Class Clazz = Class.ForName(Type);
					Constructor Constructor = Clazz.GetConstructor();
					Object Object = Constructor.NewInstance();
					Item Item = (Item) Object;
					Item.Load(Reader);
					this.Item = Item;
				} catch (Exception) {
					Dungeon.ReportException(E);
				}
			} 

			Fake = false;

			if (this.Body != null) {
				this.Body.SetTransform(this.X, this.Y, 0f);
			} 
		}
	}
}
