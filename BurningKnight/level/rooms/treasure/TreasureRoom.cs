using BurningKnight.entity.chest;
using BurningKnight.entity.creature.mob;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public override void Paint(Level level) {
			PlaceChest(level, GetCenter());
		}

		public override void PaintFloor(Level level) {
			Painter.Fill(level, this, Tiles.RandomWall());
			Painter.Fill(level, this, 1, Tile.FloorD);	
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}

		protected void PlaceChest(Level level, Vector2 where) {
			var chance = GameSave.GetFloat("mimic_chance") * 100;

			if (true || Random.Chance(chance)) {
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
		}

		public override bool ShouldSpawnMobs() {
			return Random.Chance(10);
		}
	}
}