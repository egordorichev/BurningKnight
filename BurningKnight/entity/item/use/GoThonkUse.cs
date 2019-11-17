using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class GoThonkUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			var room = entity.GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			var enemies = room.Tagged[Tags.Mob];

			if (enemies.Count == 0) {
				return;
			}

			var attempt = 0;

			do {
				var enemy = enemies[Rnd.Int(enemies.Count)];

				if (enemy.GetComponent<BuffsComponent>().Add(new FrozenBuff {
						Duration = 30
				}) != null) {
					if (!enemy.HasComponent<DialogComponent>()) {
						enemy.AddComponent(new DialogComponent());
					}
					
					enemy.GetComponent<DialogComponent>().Start("mob_0");
					break;
				}
			} while (attempt++ < 99);
		}
	}
}