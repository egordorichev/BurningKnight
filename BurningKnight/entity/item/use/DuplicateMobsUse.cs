using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using Lens.entity;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.use {
	public class DuplicateMobsUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			
			var room = entity.GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			// Copy to array, because we are going to change the list in the loop
			var mobs = room.Tagged[Tags.Mob].ToArray();

			foreach (var mob in mobs) {
				try {
					var m = (Mob) Activator.CreateInstance(mob.GetType());
					entity.Area.Add(m);
					m.Center = mob.Center;

					if (!m.SpawnsNearWall()) {
						m.Center += Random.Vector(-8, 8);
					}
				} catch (Exception e) {
					Log.Error(e);
				}
			}
		}
	}
}