using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.key;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.entities.shop;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms {
	public class HandmadeRoom : RegularRoom {
		public static TiledMap Map;
		public static MapLayer Rooms;
		public static MapLayer Objects;
		public static TiledMapTileLayer Tiles;
		public static Dictionary<string, RoomData> Datas = new Dictionary<>();
		public static Dictionary<string, List<MapObject>> Sorted = new Dictionary<>();
		private static bool Inited;
		public RoomData Data;
		private string Id;

		public HandmadeRoom(string Id) {
			Data = Datas.Get(Id);

			if (Data == null) throw new RuntimeException("Handmade " + Id + " does not exist!");

			this.Id = Id;
		}

		public HandmadeRoom() {
			this("grid_room");
		}

		public static void Init() {
			if (Inited) return;

			Inited = true;
			Log.Info("Loading handmade rooms");
			Map = new TmxMapLoader().Load("maps/main.tmx");
			MapLayers Layers = Map.GetLayers();

			foreach (MapLayer Layer in Layers)
				if (Layer.GetName().Equals("tiles"))
					Tiles = (TiledMapTileLayer) Layer;
				else if (Layer.GetName().Equals("rooms"))
					Rooms = Layer;
				else if (Layer.GetName().Equals("objects")) Objects = Layer;

			foreach (MapObject Object in Rooms.GetObjects()) {
				if (Object.GetName().Equals("sub")) continue;

				if (Object is RectangleMapObject) {
					Rectangle Rect = ((RectangleMapObject) Object).GetRectangle();
					var Data = new RoomData();
					Data.Rect = Rect;
					Data.W = (byte) (Rect.Width / 16);
					Data.H = (byte) (Rect.Height / 16);
					Data.Data = new byte[Data.W * Data.H];
					var Rx = Rect.X / 16;
					var Ry = Rect.Y / 16;

					for (var X = 0; X < Data.W; X++)
					for (var Y = 0; Y < Data.H; Y++) {
						TiledMapTileLayer.Cell Cell = Tiles.GetCell(Rx + X, Ry + Y);
						Data.Data[X + Y * Data.W] = Cell == null ? 9 : (byte) Cell.GetTile().GetId();
					}

					Datas.Put(Object.GetName(), Data);
					List<MapObject> List = new List<>();
					Sorted.Put(Object.GetName(), List);

					foreach (MapObject O in Objects.GetObjects())
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

			foreach (MapObject Object in Rooms.GetObjects()) {
				if (!(Object is RectangleMapObject) || !Object.GetName().Equals("sub")) continue;

				foreach (RoomData Data in Datas.Values()) {
					Rectangle R = ((RectangleMapObject) Object).GetRectangle();

					if (Data.Rect.Overlaps(R)) {
						var Rect = new Rect();
						Rect.Left = (int) (R.X - Data.Rect.X) / 16;
						Rect.Top = (int) (R.Y - Data.Rect.Y) / 16;
						Rect.Resize((int) R.GetWidth() / 16, (int) R.GetHeight() / 16);
						Data.Sub.Add(Rect);

						break;
					}
				}
			}
		}

		public static void Destroy() {
			if (!Inited) return;

			Map.Dispose();
		}

		public void AddSubRooms(List Rooms) {
			Log.Error("Adding sub rooms " + Data.Sub.Size());

			foreach (Rect Sub in Data.Sub) {
				var Room = new SubRoom();
				Room.Left = Sub.Left + 1;
				Room.Top = Sub.Top + 1;
				Room.Resize(Sub.GetWidth() - 1, Sub.GetHeight() - 1);
				Rooms.Add(Room);
			}
		}

		protected void Parse(List List) {
			float X = Left * 16;
			float Y = Top * 16;

			foreach (MapObject O in List) {
				string Name = O.GetName();

				if (Name == null) continue;

				Rectangle Rect = ((RectangleMapObject) O).GetRectangle();

				if (Name.StartsWith("sk_")) {
					var Id = Name.Replace("sk_", "");
					var Trader = new Trader();
					Trader.Id = Id;
					Trader.X = X + Rect.X + 16;
					Trader.Y = Y + Rect.Y + 16 - 8;

					if (Version.Debug) Trader.Saved = true;

					Dungeon.Area.Add(Trader.Add());
				}
				else if (Name.StartsWith("sp_")) {
					var Id = Name.Replace("sp_", "");
					var Trader = new Upgrade();
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
				}
				else if (Name.Equals("hat")) {
					var Trader = new HatSelector();
					Trader.X = X + Rect.X + 16;
					Trader.Y = Y + Rect.Y + 16 - 8;
					Dungeon.Area.Add(Trader.Add());
				}
				else if (Name.Equals("start")) {
					var Spawn = new Spawn();
					Spawn.X = X + Rect.X + 16;
					Spawn.Y = Y + Rect.Y + 16;
					Spawn.Room.Set(this);
					Dungeon.Area.Add(Spawn.Add());
				}
				else if (Name.Equals("tutorial_chest")) {
					Chest Chest = new WoodenChest();
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = true;
					Dungeon.Area.Add(Chest.Add());
				}
				else if (Name.Equals("heal_chest")) {
					Chest Chest = new WoodenChest();
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = false;
					Dungeon.Area.Add(Chest.Add());
				}
				else if (Name.Equals("gun_chest")) {
					Chest Chest = new WoodenChest();
					Chest.SetItem(new Revolver());
					Chest.X = X + Rect.X + 16;
					Chest.Y = Y + Rect.Y + 16;
					Chest.Locked = false;
					Dungeon.Area.Add(Chest.Add());
				}
				else if (Name.Equals("key")) {
					var Key = new ItemHolder();
					Key.X = X + Rect.X + 16;
					Key.Y = Y + Rect.Y + 16;
					Key.SetItem(new KeyC());
					Dungeon.Area.Add(Key.Add());
				}
				else if (Name.Equals("prop_tree")) {
					var Tree = new Tree();
					Tree.X = X + Rect.X + 16;
					Tree.Y = Y + Rect.Y + 16;
					Dungeon.Area.Add(Tree.Add());
				}
				else if (Name.Equals("prop_stone") || Name.Equals("prop_big_stone") || Name.Equals("prop_high_stone")) {
					var Tree = new Stone();
					Tree.X = X + Rect.X + 16;
					Tree.Y = Y + Rect.Y + 16;
					Tree.Sprite = Name;
					Dungeon.Area.Add(Tree.Add());
				}
				else if (Name.Equals("boss")) {
					if (BurningKnight.Instance == null) Dungeon.Area.Add(new BurningKnight().Add());

					BurningKnight.Instance.Become("unactive");
					BurningKnight.Instance.Tp(X + Rect.X + 16, Y + Rect.Y + 16);
				}
				else if (Name.Equals("carpet") || Name.Equals("stand") || Name.Equals("target") || Name.Equals("shields") || Name.Equals("maniken") || Name.Equals("bone") || Name.Equals("bat") || Name.Equals("frog") || Name.Equals("skull") ||
				         Name.Equals("blood") || Name.Equals("frame_a") || Name.Equals("frame_b")) {
					var Prop = new ShopProp();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Prop.Sprite = "shop-" + Name;
					Dungeon.Area.Add(Prop.Add());
				}
				else if (Name.Equals("table") || Name.Equals("table_2") || Name.Equals("cauldron") || Name.Equals("shelf")) {
					var Prop = new SolidShopProp();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Prop.Sprite = "shop-" + Name;
					Dungeon.Area.Add(Prop.Add());
				}
				else if (Name.Equals("blocker")) {
					var Prop = new Blocker();
					Prop.X = X + Rect.X + 16;
					Prop.Y = Y + Rect.Y + 16 - 8;
					Dungeon.Area.Add(Prop.Add());
				}
				else {
					Log.Error("Unknown entity " + Name);
				}
			}
		}

		public override bool CanConnect(Point P) {
			float Y = P.Y;
			float X = P.X;

			if (Y == Top)
				Y += 1;
			else if (Y == Bottom) Y -= 1;

			if (X == Left)
				X += 1;
			else if (X == Right) X -= 1;

			var I = (int) (X - Left - 1 + (Y - Top - 1) * Data.W);
			var T = Data.Data[I];

			if (T == 6 || T == 8 || T == 9) return false;

			return base.CanConnect(P);
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			for (var X = 0; X < Data.W; X++)
			for (var Y = 0; Y < Data.H; Y++) {
				var Tt = Terrain.FLOOR_A;
				var T = Data.Data[X + Y * Data.W];
				var Dr = false;

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
						var Xx = X + Left + 1;
						var Yy = Y + Left + 1;
						Dr = true;
						var Door = new Door(Xx, Yy, Data.Data[X - 1 + Y * Data.W] != 8);
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
						var Exit = new Exit();
						Exit.X = (X + Left + 1) * 16;
						Exit.Y = (Y + Top + 1) * 16 - 8;
						Dungeon.Level.Set(Left + X + 1, Top + Y + 1, Terrain.FLOOR_B);
						Dungeon.Level.Set(Left + X + 1, Top + Y + 1, Terrain.EXIT);
						Dungeon.Area.Add(Exit.Add());

						break;
					}

					case 14: {
						Dr = true;
						var Entrance = new Entrance();
						Entrance.X = (X + Left + 1) * 16 + 1;
						Entrance.Y = (Y + Top + 1) * 16 - 4;
						Dungeon.Area.Add(Entrance.Add());

						break;
					}

					default: {
						Log.Error("Unknown tile " + T);
					}
				}

				if (!Dr) Level.Set(Left + 1 + X, Top + 1 + Y, Tt);
			}

			Parse(Sorted.Get(Id));
		}

		public override int GetMinHeight() {
			return Data.H + 2;
		}

		public override int GetMinWidth() {
			return Data.W + 2;
		}

		public override int GetMaxWidth() {
			return Data.W + 3;
		}

		public override int GetMaxHeight() {
			return Data.H + 3;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.ALL) return 16;

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) return 1;

			return 0;
		}

		public static class RoomData {
			public byte[] Data;
			public byte H;
			public Rectangle Rect;
			public List<Rect> Sub = new List<>();
			public byte W;
		}
	}
}