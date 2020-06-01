using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.twitch.happening {
	public class TeleportHappening : Happening {
		private RoomType[] types;

		public TeleportHappening(params RoomType[] t) {
			types = t;
		}
		
		public override void Happen(Player player) {
			var list = new List<Room>();

			foreach (var rm in player.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				foreach (var t in types) {
					if (room.Type == t) {
						list.Add(room);
						break;
					}
				}
			}

			if (list.Count == 0) {
				return;
			}

			var r = list[Rnd.Int(list.Count)];
			
			AnimationUtil.TeleportAway(player, () => {
				player.Center = r.GetRandomFreeTile() * 16 + new Vector2(8);
				Camera.Instance.Jump();

				AnimationUtil.TeleportIn(player);
			});
		}
	}
}