using System;
using BurningKnight.level.entities.chest;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretChestRoom : SecretRoom {
		public override void Paint(Level level) {
			Entity chest = null;

			if (Rnd.Chance(10)) {
				chest = new ProtoChest();
			} else {
				try {
					chest = ChestRegistry.PlaceRandom(Vector2.Zero, level.Area);
				} catch (Exception e) {
					Log.Error(e);
					chest = new WoodenChest();
				}
			}

			level.Area.Add(chest);
			chest.BottomCenter = GetCenter() * 16 + new Vector2(8, 8);
		}
	}
}