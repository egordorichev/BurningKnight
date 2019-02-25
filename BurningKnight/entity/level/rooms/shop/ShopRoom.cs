using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.key;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.features;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.level.save;
using BurningKnight.entity.pool;
using BurningKnight.entity.pool.item;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.shop {
	public class ShopRoom : LockedRoom {
		private bool DoublePrice;
		private bool Hd;

		public ShopRoom() {
			Hd = Random.Chance(30);
		}

		protected void PlaceItems() {
			var C = GetItemCount();
			Shopkeeper Npc = null;

			switch (Random.NewInt(4)) {
				case 0:
				case 1: {
					Npc = new Shopkeeper();

					break;
				}

				case 2: {
					Npc = new BlueShopkeeper();

					break;
				}

				case 3: {
					Npc = new OrangeShopkeeper();

					break;
				}
			}

			switch (Random.NewInt(4)) {
				case 0: {
					PaintWeapon(C);

					break;
				}

				case 1: {
					PaintAccessory(C);

					break;
				}

				case 2:
				case 3: {
					PaintMixed(C);

					break;
				}
			}

			var Point = GetSpawn();
			Npc.X = Point.X * 16;
			Npc.Generate();
			Npc.Y = Point.Y * 16;
			LevelSave.Add(Npc);
			Dungeon.Area.Add(Npc);

			if (Random.Chance(30))
				for (var I = 0; I < Random.NewInt(1, 4); I++) {
					var Holder = new ItemHolder(new Gold());
					Holder.GetItem().Generate();
					var P = GetRandomFreeCell();
					Holder.X = P.X * 16 + (16 - Holder.GetItem().GetSprite().GetRegionWidth()) / 2;
					Holder.Y = P.Y * 16 + (16 - Holder.GetItem().GetSprite().GetRegionHeight()) / 2;
					Dungeon.Area.Add(Holder);
					LevelSave.Add(Holder);
				}
		}

		protected Point GetSpawn() {
			return GetRandomFreeCell();
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Hd) {
				foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.SECRET);

				Hidden = true;
			}
		}

		public override bool CanConnect(Room R) {
			if (Hd && R is ConnectionRoom) return false;

			return base.CanConnect(R);
		}

		protected int GetItemCount() {
			return (GetWidth() - 1) / 2;
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W + 1;
		}

		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMaxWidth() {
			return 16;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 9;
		}

		private void PaintArmor(int C) {
			List<Item> Items = new List<>();

			for (var I = 0; I < C; I++) Items.Add(ShopHatPool.Instance.Generate());

			PlaceItems(Items);
		}

		private void PaintWeapon(int C) {
			List<Item> Items = new List<>();
			Pool<Item> Pool = Chest.MakePool(ItemRegistry.Quality.ANY, false, false);

			if (Random.Chance(50)) {
				var Bomb = new Bomb();
				Bomb.Generate();
				C -= 1;
				Items.Add(Bomb);
			}

			if (Random.Chance(50)) {
				Items.Add(new KeyC());
				C -= 1;
			}

			for (var I = 0; I < C; I++) Items.Add(Pool.Generate());

			PlaceItems(Items);
		}

		private void PaintAccessory(int C) {
			List<Item> Items = new List<>();
			var Pool = Chest.MakePool(ItemRegistry.Quality.ANY, false, false);

			if (Random.Chance(50)) {
				var Bomb = new Bomb();
				Bomb.Generate();
				C -= 1;
				Items.Add(Bomb);
			}

			if (Random.Chance(50)) {
				Items.Add(new KeyC());
				C -= 1;
			}

			for (var I = 0; I < C; I++) Items.Add((Item) Pool.Generate());

			PlaceItems(Items);
		}

		private void PaintMixed(int C) {
			List<Item> Items = new List<>();
			var Weapon = Chest.MakePool(ItemRegistry.Quality.ANY, true, false);
			var Accessory = Chest.MakePool(ItemRegistry.Quality.ANY, false, false);

			if (Random.Chance(50)) {
				var Bomb = new Bomb();
				Bomb.Generate();
				C -= 1;
				Items.Add(Bomb);
			}

			if (Random.Chance(50)) {
				Items.Add(new KeyC());
				C -= 1;
			}

			for (var I = 0; I < C; I++) {
				Item Item = null;

				switch (Random.NewInt(2)) {
					case 0: {
						Item = (Item) Weapon.Generate();

						break;
					}

					case 1: {
						Item = (Item) Accessory.Generate();

						break;
					}
				}

				if (Item == null) {
					Log.Error("Null item result!");
					Item = new KeyB();
				}

				Items.Add(Item);
			}

			PlaceItems(Items);
		}

		protected void PlaceItems(List Items) {
			var I = 0;

			for (var X = 2; X < Items.Size() * 2; X += 2) {
				PlaceItem(Items.Get(I), (Left + X) * 16 + 1, (Top + 3) * 16 - 4);
				I++;
			}
		}

		protected void PlaceItem(Item Item, int X, int Y) {
			Item.Generate();
			var Slab = new Slab();
			Slab.X = X + 1;
			Slab.Y = Y - 4;
			LevelSave.Add(Slab);
			Dungeon.Area.Add(Slab);
			var Holder = new ItemHolder(Item);
			Holder.X = X + (16 - Holder.W) / 2;
			Holder.Y = Y + (16 - Holder.H) / 2;
			Holder.GetItem().Shop = true;

			if (DoublePrice) Holder.GetItem().Price += 5;

			if (Player.Instance != null) {
				var Cn = Player.Instance.Sales;

				for (var J = 0; J < Cn; J++) Holder.Sale();
			}

			LevelSave.Add(Holder);
			Dungeon.Area.Add(Holder);
		}
	}
}