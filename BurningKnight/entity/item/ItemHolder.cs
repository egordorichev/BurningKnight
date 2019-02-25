using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.shop;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class ItemHolder : SaveableEntity {
		public static List<ItemHolder> All = new List<>();
		private bool Added;
		public float Al;
		public bool Auto;
		protected Body Body;
		public bool CreateBody;
		protected bool Fake = false;
		private float Fall = 1f;
		public bool Falling;
		private int Hh;
		private int Hw;
		protected Item Item;
		protected float Last;
		public ItemPrice Price;
		protected float StartX;
		protected float StartY;
		private float Sz = 1f;
		private float Z;

		public ItemHolder(Item Item) {
			SetItem(Item);
		}

		public ItemHolder() {
		}

		public void SetItem(Item Item) {
			this.Item = Item;
			Body = World.RemoveBody(Body);

			if (Item == null) return;

			if (this.Item.GetSprite() == null) return;

			CreateBody = true;
			W = Item.GetSprite().GetRegionWidth();
			H = Item.GetSprite().GetRegionHeight();
		}

		protected Body CreateSimpleBody(int X, int Y, int W, int H, BodyDef.BodyType Type, bool Sensor) {
			Hw = W;
			Hh = H;

			return World.CreateSimpleBody(this, X, Y, W, H, Type, Sensor);
		}

		public void RandomVelocity() {
			var A = Random.NewFloat(Math.PI * 2);
			var F = Random.NewFloat(60f, 150f);
			Velocity.X = Math.Cos(A) * F;
			Velocity.Y = Math.Sin(A) * F;
		}

		public void VelocityToMouse() {
			float Dx = Input.Instance.WorldMouse.X - this.X;
			float Dy = Input.Instance.WorldMouse.Y - this.Y;
			var A = (float) Math.Atan2(Dy, Dx);
			Velocity.X = Math.Cos(A) * 100f;
			Velocity.Y = Math.Sin(A) * 100f;
		}

		public void Sale() {
			Item.Sale = true;
			Item.Price = (int) Math.Max(0.0, Math.Floor(Item.Price / 2));
			Added = false;

			if (Price != null) Price.Done = true;
		}

		public bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Chest) return false;

			if (Item is Gold && !(Entity is Player)) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override void Update(float Dt) {
			if (Falling) {
				Fall -= Dt;

				if (Fall <= 0) Done = true;
			}

			if (CreateBody) {
				if (!Fake) {
					Body = CreateSimpleBody(-2, -2, Item.GetSprite().GetRegionWidth() + 4, Item.GetSprite().GetRegionHeight() + 4, BodyDef.BodyType.DynamicBody, false);
					Body.SetTransform(this.X, this.Y + Z, 0f);
				}

				CreateBody = false;
			}

			if (Item == null) return;

			if (Item.Shop && !Added) {
				Added = true;
				var Price = new ItemPrice();
				Item.Price = Item.GetPrice();
				Item.Price = Math.Max(1, Item.Price + Random.NewInt(-3, 3));
				Price.X = this.X + W / 2;
				Price.Y = this.Y - 6f - (16 - H) / 2;
				Price.Price = Item.Price;
				Price.Sale = Item.Sale;
				this.Price = Price;
				Dungeon.Area.Add(Price);
			}

			if (!Item.Shop && !Falling) {
				var Found = false;
				var X = Math.Floor(this.X / 16) - 1;

				while (X < Math.Ceil((this.X + Hw + 8) / 16)) {
					var Y = Math.Floor(this.Y / 16) - 1;

					while (Y < Math.Ceil((this.Y + 16f + Hh) / 16)) {
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

						if (Found) break;

						Y++;
					}

					if (Found) break;

					X++;
				}

				if (!Found) {
					Log.Error("Fallling");
					Falling = true;
				}
			}

			T += Dt;
			Last += Dt;

			if (Done) return;

			if (Last > 0.5f) {
				Last = 0f;
				Spark.RandomOn(this.X, this.Y, Hw, Hh);
			}

			base.Update(Dt);

			if (Body != null) {
				if (Item.Shop) {
					World.CheckLocked(Body).SetTransform(this.X, this.Y + Z, 0f);
				}
				else {
					this.X = Body.GetPosition().X;
					this.Y = Body.GetPosition().Y - Z;
				}
			}

			Velocity.Mul(0.9f);

			if (!InGameState.Dark && Item is Gold && Item.AutoPickup && !Done) {
				Room Room = Dungeon.Level.FindRoomFor(this.X + W / 2, this.Y + H / 2);

				if (Room != null && !(Room is ShopRoom) && Room == Player.Instance.Room && !Room.Hidden) {
					var Dx = Player.Instance.X + Player.Instance.W / 2 - this.X - W / 2;
					var Dy = Player.Instance.Y + Player.Instance.H / 2 - this.Y - H / 2;
					var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
					var F = 20f;
					Velocity.X += Dx / D * F;
					Velocity.Y += Dy / D * F;
				}
			}

			Sz = Math.Max(1f, Sz - Sz * Dt);

			if (Velocity.Len() <= 0.1f) {
				Velocity.Mul(0f);
				this.X = Math.Round(this.X);
				this.Y = Math.Round(this.Y);
				Z += Math.Cos(T * 1.7f) / 5f * (Sz / 2) * Dt * 60.0;
				Z = MathUtils.Clamp(0f, 5f, Z);

				if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y + Z, 0f);
			}

			Item.Update(Dt);

			if (Body != null) Body.SetLinearVelocity(Velocity);
		}

		public override void Init() {
			base.Init();
			StartX = X;
			StartY = Y;
			T = Random.NewFloat(32f);
			Last = Random.NewFloat(1f);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0f);

			All.Add(this);
		}

		public override void Destroy() {
			base.Destroy();

			if (Price != null) {
				Price.Remove();
				Price = null;
			}

			Body = World.RemoveBody(Body);
			All.Remove(this);
		}

		public override void Render() {
			if (Item == null) return;

			TextureRegion Sprite = Item.GetSprite();
			var A = Math.Cos(T * 3f) * 8f * Sz;
			var Sy = (1f + Math.Sin(T * 2f) / 10f) * Fall;
			Graphics.Batch.End();
			float Dt = Gdx.Graphics.GetDeltaTime();
			Al = MathUtils.Clamp(0f, 1f, Al + ((Player.Instance.PickupFx != null && Player.Instance.PickupFx.Item == this ? 1 : 0) - Al) * Dt * 10f);

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
						Graphics.Render(Sprite, this.X + W / 2 + Xx, this.Y + Z + H / 2 + Yy, A, W / 2, H / 2, false, false, Fall, Sy);

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
			}

			WeaponBase.Shader.Begin();
			WeaponBase.Shader.SetUniformf("a", 1f);
			WeaponBase.Shader.SetUniformf("gray", 1f);
			WeaponBase.Shader.SetUniformf("time", Dungeon.Time + T);
			WeaponBase.Shader.End();
			Graphics.Batch.SetShader(WeaponBase.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, this.X + W / 2, this.Y + Z + H / 2, A, W / 2, H / 2, false, false, Fall, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W * Fall, H * Fall, Z);
		}

		public Item GetItem() {
			return Item;
		}

		public void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature) Tween.To(new Tween.Task(4f, 0.3f) {

		public override float GetValue() {
			return Sz;
		}

		public override void SetValue(float Value) {
			Sz = Value;
		}
	});
}

}
public override void Save(FileWriter Writer) {
base.Save(Writer);
Writer.WriteBoolean(this.Item != null);
if (this.Item != null) {
Writer.WriteString(this.Item.GetClass().GetName());
this.Item.Save(Writer);
}
}
public override void Load(FileReader Reader) {
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