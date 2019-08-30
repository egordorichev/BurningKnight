using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public override void Paint(Level level) {
			var c = GetCenter() * 16;
			
			PlaceStand(level, c - new Vector2(32, 0));
			PlaceStand(level, c);
			PlaceStand(level, c + new Vector2(32, 0));
		}

		protected void PlaceStand(Level level, Vector2 where) {
			var stand = new SingleChoiceStand();
			level.Area.Add(stand);
			stand.Position = where;
			
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Chest), level.Area), null);
		}

		public override void PaintFloor(Level level) {
			FloorRegistry.Paint(level, this, -1, true);
			Painter.Rect(level, this, 1, Tile.FloorD);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Enemy;
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