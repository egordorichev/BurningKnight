using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.item.use.parent;
using BurningKnight.entity.room;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class TeleportUse : DoWithTagUse {
		protected override void DoAction(Entity entity, Item item, List<Entity> entities) {
			var rooms = entity.Area.Tagged[Tags.Room];

			if (rooms.Count < 2) {
				return;
			}
			
			foreach (var e in entities) {
				var room = e.GetComponent<RoomComponent>().Room;
				var newRoom = (Room) Random.Element<Entity>(rooms, r => r != room);

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
}