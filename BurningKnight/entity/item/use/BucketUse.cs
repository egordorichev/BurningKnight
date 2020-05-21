using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class BucketUse : ItemUse {
		private bool water;
		private bool snow;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (water) {
				// ReplaceItem(entity, "bk:bucket");
			} else if (snow) {
				
			} else if (Run.Level.Biome is IceBiome) {
				var x = (int) Math.Floor(entity.CenterX / 16);
				var y = (int) Math.Floor(entity.CenterY / 16);

				for (var xx = -1; xx <= 1; xx++) {
					for (var yy = -1; yy <= 1; yy++) {
						var d = Math.Sqrt(xx * xx + yy * yy);

						if (d <= 1 && Run.Level.IsInside(x + xx, y + yy) && Run.Level.Get(x + xx, y + yy) == Tile.WallA) {
							ReplaceItem(entity, "bk:snow_bucket");
							break;
						}
					}
				}
			}
		}

		private void ReplaceItem(Entity entity, string id) {
			var c = entity.GetComponent<ActiveItemComponent>();
			var i = c.Item;
				
			c.Drop();
			i.Done = true;

			entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(id, entity.Area));
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			water = settings["wt"].Bool(false);
			snow = settings["snw"].Bool(true);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("Water Bucket", "wt", false);
			root.Checkbox("Snow Bucket", "snw", false);
		}
	}
}