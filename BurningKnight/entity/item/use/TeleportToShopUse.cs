using BurningKnight.entity.component;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class TeleportToShopUse : ItemUse {
		public override void Use(Entity e, Item item) {
			base.Use(e, item);
			
			var rooms = e.Area.Tagged[Tags.Room];

			if (rooms.Count < 2) {
				return;
			}

			var room = e.GetComponent<RoomComponent>().Room;
			var newRoom = (Room) Rnd.Element<Entity>(rooms, r => r != room && r is Room rm && rm.Type == RoomType.Shop);

			if (newRoom != null) {
				AnimationUtil.TeleportAway(e, () => {
					e.Center = newRoom.GetRandomFreeTile() * 16 + new Vector2(8);
					Camera.Instance.Jump();
					AnimationUtil.TeleportIn(e);
				});
			}
		}
	}
}