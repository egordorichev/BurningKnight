using BurningKnight.core.entity.creature.npc;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.entities.shop;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms {
	public class HandmadeRoom : RegularRoom {
		public static class RoomData {
			public byte[] Data;
			public byte W;
			public byte H;
			public Rectangle Rect;
			public List<Rect> Sub = new List<>();
		}

		public static TiledMap Map;
		public static MapLayer Rooms;
		public static MapLayer Objects;
		public static TiledMapTileLayer Tiles;
		public static Dictionary<string, RoomData> Datas = new Dictionary<>();
		public static Dictionary<string, List<MapObject>> Sorted = new Dictionary<>();
		private static bool Inited;
		public RoomData Data;
		private string Id;

		public static Void Init() {
			if (Inited) {
				return;
			} 

			Inited = true;
			Log.Info("Loading handmade rooms");
			Map = new TmxMapLoader().Load("maps/main.tmx");
			MapLayers Layers = Map.GetLayers();

			foreach (MapLayer Layer in Layers) {
				if (Layer.GetName().Equals("tiles")) {
					Tiles = (TiledMapTileLayer) Layer;
				} else if (Layer.GetName().Equals("rooms")) {
					Rooms = Layer;
				} else if (Layer.GetName().Equals("objects")) {
					Objects = Layer;
				} 
			}

			foreach (MapObject Object in Rooms.GetObjects()) {
				if (Object.GetName().Equals("sub")) {
					continue;
				} 

				if (Object is RectangleMapObject) {
					Rectangle Rect = ((RectangleMapObject) Object).GetRectangle();
					RoomData Data = new RoomData();
					Data.Rect = Rect;
					Data.W = (byte) (Rect.Width / 16);
					Data.H = (byte) (Rect.Height / 16);
					Data.Data = new byte[Data.W * Data.H];
					int Rx = (int) (Rect.X / 16);
					int Ry = (int) (Rect.Y / 16);

					for (int X = 0; X < Data.W; X++) {
						for (int Y = 0; Y < Data.H; Y++) {
							TiledMapTileLayer.Cell Cell = Tiles.GetCell(Rx + X, Ry + Y);
							Data.Data[X + Y * Data.W] = Cell == null ? 9 : (byte) Cell.GetTile().GetId();
						}
					}

					Datas.Put(Object.GetName(), Data);
					List<MapObject> List = new List<>();
					Sorted.Put(Object.GetName(), List);

					foreach (MapObject O in Objects.GetObjects()) {
						if (O is RectangleMapObject) {
							RectangleMapObject R = (RectangleMapObject) O;
							Rectangle Rc = R.GetRectangle();

							if (Rect.Contains(Rc)) {
								Rc.SetX(Rc.GetX() - Rect.GetX());
								Rc.SetY(Rc.GetY() - Rect.GetY());
								List.Add(O);
							} 
						} 
					}
				} 
			}

			foreach (MapObject Object in Rooms.GetObjects()) {
				if (!(Object is RectangleMapObject) || !Object.GetName().Equals("sub")) {
					continue;
				} 

				foreach (RoomData Data in Datas.Values()) {
					Rectangle R = ((RectangleMapObject) Object).GetRectangle();

					if (Data.Rect.Overlaps(R)) {
						Rect Rect = new Rect();
						Rect.Left = (int) (R.X - Data.Rect.X) / 16;
						Rect.Top = (int) (R.Y - Data.Rect.Y) / 16;
						Rect.Resize((int) R.GetWidth() / 16, (int) R.GetHeight() / 16);
						Data.Sub.Add(Rect);

						break;
					} 
				}
			}
		}

		public static Void Destroy() {
			if (!Inited) {
				return;
			} 

			Map.Dispose();
		}

		public Void AddSubRooms(List Rooms) {
			Log.Error("Adding sub rooms " + this.Data.Sub.Size());

			foreach (Rect Sub in this.Data.Sub) {
				SubRoom Room = new SubRoom();
				Room.Left = Sub.Left + 1;
				Room.Top = Sub.Top + 1;
				Room.Resize(Sub.GetWidth() - 1, Sub.GetHeight() - 1);
				Rooms.Add(Room);
			}
		}

		public HandmadeRoom(string Id) {
			this.Data = Datas.Get(Id);

			if (this.Data == null) {
				throw new RuntimeException("Handmade " + Id + " does not exist!");
			} 

			this.Id = Id;
		}

		protected Void Parse(List List) {
			float X = this.Left * 16;
			float Y = this.Top * 16;

			foreach (MapObject O in List) {
				string Name = O.GetName();

				if (Name == null) {
					continue;
				} 

				Rectangle Rect = ((RectangleMapObject) O).GetRectangle();

				if (Name.StartsWith("sk_")) {
					string Id = Name.Replace("sk_", "");
					Trader Trader = new Trader();
					Trader.Id = Id;
					Trader.X = X + Rect.X + 16;
					Trader.Y = Y + Rect.Y + 16 - 8;

					if (Version.Debug) {
						Trader.Saved = true;
					} 

					Dungeon.Area.Add(Trader.Add());
				} else if (Name.StartsWith("sp_")) {
					string Id = Name.Replace("sp_", "");
					Upgrade Trader = new Upgrade();
					Trader.X = X + Rect.X + 16;
					Trader.Y = Y + Rect.Y + 16 - 8;
					Trader.Idd = Id;

					switch (Id) {
						case "a": {
							Trader.Type = Upgrade.Type.ACCESSORY;

							break;
						}

						case "d": {
							Trader.Type = Upgrade.Type.WEAPON;

							break;
						}

						case "c": {
							Trader.Type = Upgrade.Type.CONSUMABLE;

							break;
						}

						case "h": {
							Trader.Type = Upgrade.Type.DECOR;

							break;
						}
					}

					Dungeon.Area.Add(Trader.Add());
				} else if (Name.Equals("hat")) {
					HatSelector Trader = new HatSelector();
					Trader.X = X + Rect.X + 16;
					Trader.Y = Y + Rect.Y + 16 - 8;
					Dungeon.Area.Add(Trader.Add());
				} else if (Name.Equals("start")) {
					Spawn Spawn = new Spawn();
					Spawn.X = X + Rect.X + 16;
					Spawn.Y = Y + Rect.Y + 16;
					Spawn.Room.Set(this);
					Dungeon.Area.Add(Spawn.Add());
				} else if (Name.Equals("tutorial_chest")) {
					Chest Chest = new WoodenChest();
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = true;
					Dungeon.Area.Add(Chest.Add());
				} else if (Name.Equals("heal_chest")) {
					Chest Chest = new WoodenChest();
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = false;
					Dungeon.Area.Add(Chest.Add());
				} else if (Name.Equals("gun_chest")) {
					Chest Chest = new WoodenChest();
					Chest.SetItem(new Revolver());
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = false;
					Dungeon.Area.Add(Chest.Add());
				} else if (Name.Equals("key")) {
					ItemHolder Key = new ItemHolder();
					Key.X = X + Rect.X + 16;
					Key.Y = Y + Rect.Y + 16;
					Key.SetItem(new KeyC());
					Dungeon.Area.Add(Key.Add());
				} else if (Name.Equals("prop_tree")) {
					Tree Tree = new Tree();
					Tree.X = X + Rect.X + 16;
					Tree.Y = Y + Rect.Y + 16;
					Dungeon.Area.Add(Tree.Add());
				} else if (Name.Equals("prop_stone") || Name.Equals("prop_big_stone") || Name.Equals("prop_high_stone")) {
					Stone Tree = new Stone();
					Tree.X = X + Rect.X + 16;
					Tree.Y = Y + Rect.Y + 16;
					Tree.Sprite = Name;
					Dungeon.Area.Add(Tree.Add());
				} else if (Name.Equals("boss")) {
					if (creature.mob.boss.BurningKnight.Instance == null) {
						Dungeon.Area.Add(new creature.mob.boss.BurningKnight().Add());
					} 

					creature.mob.boss.BurningKnight.Instance.Become("unactive");
					creature.mob.boss.BurningKnight.Instance.Tp(X + Rect.X + 16, Y + Rect.Y + 16);
				} else if (Name.Equals("carpet") || Name.Equals("stand") || Name.Equals("target") || Name.Equals("shields") || Name.Equals("maniken") || Name.Equals("bone") || Name.Equals("bat") || Name.Equals("frog") || Name.Equals("skull") || Name.Equals("blood") || Name.Equals("frame_a") || Name.Equals("frame_b")) {
					ShopProp Prop = new ShopProp();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Prop.Sprite = "shop-" + Name;
					Dungeon.Area.Add(Prop.Add());
				} else if (Name.Equals("table") || Name.Equals("table_2") || Name.Equals("cauldron") || Name.Equals("shelf")) {
					SolidShopProp Prop = new SolidShopProp();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Prop.Sprite = "shop-" + Name;
					Dungeon.Area.Add(Prop.Add());
				} else if (Name.Equals("blocker")) {
					Blocker Prop = new Blocker();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Dungeon.Area.Add(Prop.Add());
				} else {
					Log.Error("Unknown entity " + Name);
				}

			}
		}

		public HandmadeRoom() {
			this("grid_room");
		}

		public override bool CanConnect(Point P) {
			float Y = P.Y;
			float X = P.X;

			if (Y == this.Top) {
				Y += 1;
			} else if (Y == this.Bottom) {
				Y -= 1;
			} 

			if (X == this.Left) {
				X += 1;
			} else if (X == this.Right) {
				X -= 1;
			} 

			int I = (int) ((X - this.Left - 1) + (Y - this.Top - 1) * this.Data.W);
			byte T = this.Data.Data[I];

			if (T == 6 || T == 8 || T == 9) {
				return false;
			} 

			return base.CanConnect(P);
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);

			for (int X = 0; X < this.Data.W; X++) {
				for (int Y = 0; Y < this.Data.H; Y++) {
					byte Tt = Terrain.FLOOR_A;
					byte T = this.Data.Data[X + Y * this.Data.W];
					bool Dr = false;

					switch (T) {
						case 1: {
							Tt = Terrain.FLOOR_A;

							break;
						}

						case 2: {
							Tt = Terrain.FLOOR_B;

							break;
						}

						case 3: {
							Tt = Terrain.FLOOR_D;

							break;
						}

						case 4: {
							Tt = Terrain.FLOOR_C;

							break;
						}

						case 5: {
							Tt = Terrain.DIRT;

							break;
						}

						case 6: {
							Tt = Terrain.LAVA;

							break;
						}

						case 7: {
							Tt = Terrain.WATER;

							break;
						}

						case 8: {
							Tt = Terrain.WALL;

							break;
						}

						case 9: {
							Tt = Terrain.CHASM;

							break;
						}

						case 10: {
							Tt = Terrain.GRASS;

							break;
						}

						case 11: {
							int Xx = X + this.Left + 1;
							int Yy = Y + this.Left + 1;
							Dr = true;
							Door Door = new Door(Xx, Yy, this.Data.Data[X - 1 + (Y * this.Data.W)] != 8);
							Door.AutoLock = true;
							Door.Add();
							Dungeon.Area.Add(Door);

							break;
						}

						case 12: {
							Tt = Terrain.CRACK;

							break;
						}

						case 13: {
							Dr = true;
							Exit Exit = new Exit();
							Exit.X = (X + this.Left + 1) * 16;
							Exit.Y = (Y + this.Top + 1) * 16 - 8;
							Dungeon.Level.Set(this.Left + X + 1, this.Top + Y + 1, Terrain.FLOOR_B);
							Dungeon.Level.Set(this.Left + X + 1, this.Top + Y + 1, Terrain.EXIT);
							Dungeon.Area.Add(Exit.Add());

							break;
						}

						case 14: {
							Dr = true;
							Entrance Entrance = new Entrance();
							Entrance.X = (X + this.Left + 1) * 16 + 1;
							Entrance.Y = (Y + this.Top + 1) * 16 - 4;
							Dungeon.Area.Add(Entrance.Add());

							break;
						}

						default:{
							Log.Error("Unknown tile " + T);
						}
					}

					if (!Dr) {
						Level.Set(this.Left + 1 + X, this.Top + 1 + Y, Tt);
					} 
				}
			}

			this.Parse(Sorted.Get(Id));
		}

		public override int GetMinHeight() {
			return this.Data.H + 2;
		}

		public override int GetMinWidth() {
			return this.Data.W + 2;
		}

		public override int GetMaxWidth() {
			return this.Data.W + 3;
		}

		public override int GetMaxHeight() {
			return this.Data.H + 3;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 16;
			} 

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 1;
			} 

			return 0;
		}
	}
}
