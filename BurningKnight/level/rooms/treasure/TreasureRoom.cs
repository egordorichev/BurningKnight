using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		private List<ItemStand> stands = new List<ItemStand>();
		
		public override void Paint(Level level) {
			var c = GetCenter() * 16;
			
			PlaceStand(level, c - new Vector2(32, 0));
			PlaceStand(level, c);
			PlaceStand(level, c + new Vector2(32, 0));

			SetupStands(level);
		}

		protected void SetupStands(Level level) {
			if (stands.Count == 0) {
				return;
			}

			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Chest));

			foreach (var s in stands) {
				s.SetItem(Items.CreateAndAdd(Items.GenerateAndRemove(pool), level.Area), null);

				if (pool.Count == 0) {
					break;
				}
			}
			
			
		}

		protected void PlaceStand(Level level, Vector2 where) {
			var stand = new SingleChoiceStand();
			level.Area.Add(stand);
			stand.Center = where + new Vector2(8, 8);
			
			stands.Add(stand);
		}

		public override void PaintFloor(Level level) {
			FloorRegistry.Paint(level, this, -1, true);
			Painter.Rect(level, this, 1, Tile.FloorD);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Locked;
			}
			
			if (Random.Chance()) {
				level.ItemsToSpawn.Add("bk:key");
			}
		}

		/*protected void PlaceChest(Level level, Vector2 where) {
			var chance = GameSave.GetFloat("mimic_chance") * 100;

			if (Random.Chance(chance)) {
				var mimic = new Mimic();
				
				level.Area.Add(mimic);

				mimic.Center = where * 16;

				Log.Info("Enjoy your mimic :)");
				return;
			}
			
			var l = Random.Chance(30);
			var chest = l ? new Chest() : new LockedChest();

			if (l) {
				level.ItemsToSpawn.Add("bk:key");
			}
			
			level.Area.Add(chest);

			chest.Center = where * 16;
			chest.GenerateLoot();
		}*/

		public override bool ShouldSpawnMobs() {
			return Random.Chance(10);
		}
	}
}