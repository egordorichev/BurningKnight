using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		private List<ItemStand> stands = new List<ItemStand>();
		
		public override void Paint(Level level) {
			var c = GetCenter() * 16;
			
			PlaceStand(level, c - new Dot(32, 0));
			PlaceStand(level, c);
			PlaceStand(level, c + new Dot(32, 0));

			SetupStands(level);
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}

		protected void SetupStands(Level level) {
			if (stands.Count == 0) {
				return;
			}

			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Treasure));

			foreach (var s in stands) {
				s.SetItem(Items.CreateAndAdd(Items.GenerateAndRemove(pool, null, true), level.Area), null);

				if (pool.Count == 0) {
					break;
				}
			}
		}

		protected void PlaceStand(Level level, Dot where) {
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
				door.Type = DoorPlaceholder.Variant.Treasure;
			}
		}

		public override bool ShouldSpawnMobs() {
			return false;// Random.Chance(10);
		}
	}
}