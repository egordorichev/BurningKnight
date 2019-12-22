using System;
using BurningKnight.level.entities;
using BurningKnight.level.entities.chest;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.shop.sub {
	public class StorageRoom : SubShopRoom {
		private static string[] infos = {
			"box_a",
			"box_b",
			"crate_a",
			"crate_b"
		};
		
		public override void Paint(Level level) {
			var spot = new Dot(Rnd.Int(Left + 2, Right - 2), Rnd.Int(Top + 2, Bottom - 2));

			try {
				var chest = (Chest) Activator.CreateInstance(ChestRegistry.Instance.Generate());
				level.Area.Add(chest);
				chest.BottomCenter = spot * 16 + new Vector2(8, 8);
			} catch (Exception e) {
				Log.Error(e);
			}

			for (var x = Left + 1; x < Right - 1; x++) {
				for (var y = Top + 1; y < Bottom - 1; y++) {
					if (Rnd.Chance(30)) {
						continue;
					}
					
					var prop = new BreakableProp {
						Sprite = infos[Rnd.Int(infos.Length)]
					};

					level.Area.Add(prop);
					prop.BottomCenter = new Vector2(x + 0.5f, y + 1f) * 16 + Rnd.Vector(-4, 4);
				}
			}
			
			Painter.Set(level, spot, Tile.FloorD);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Locked;
			}
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}