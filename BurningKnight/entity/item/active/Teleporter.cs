using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.rooms.special;

namespace BurningKnight.entity.item.active {
	public class Teleporter : ActiveItem {
		public Teleporter() {
			_Init();
		}

		protected void _Init() {
			{
				UseTime = 30f;
			}
		}

		public override void Use() {
			base.Use();

			for (var I = 0; I < 64; I++) {
				Room Room = Dungeon.Level.GetRandomRoom();

				if (Room is ConnectionRoom || Room is BossEntranceRoom || Room is NpcSaveRoom) continue;

				for (var J = 0; J < 32; J++) {
					var Point = Room.GetRandomFreeCell();

					if (Point != null) {
						Player.Instance.Poof();
						Player.Instance.Tp(Point.X * 16, Point.Y * 16);
						Player.Instance.Poof();
						Player.Instance.PlaySfx("grenade_launcher");

						return;
					}
				}
			}
		}
	}
}