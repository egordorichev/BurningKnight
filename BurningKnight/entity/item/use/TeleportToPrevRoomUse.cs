using BurningKnight.entity.events;
using BurningKnight.entity.room;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class TeleportToPrevRoomUse : ItemUse {
		private Room previous;

		public override void Use(Entity e, Item item) {
			base.Use(e, item);

			if (previous == null) {
				return;
			}
			
			AnimationUtil.TeleportAway(e, () => {
				e.Center = previous.GetRandomFreeTile() * 16 + new Vector2(8);
				Camera.Instance.Jump();
				AnimationUtil.TeleportIn(e);

				previous = null;
			});
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce && rce.Who == Item.Owner) {
				previous = rce.Old;
			}
			
			return base.HandleEvent(e);
		}
	}
}