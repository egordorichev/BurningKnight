using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.connection;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.special;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.active {
	public class Teleporter : ActiveItem {
		protected void _Init() {
			{
				UseTime = 30f;
			}
		}

		public override Void Use() {
			base.Use();

			for (int I = 0; I < 64; I++) {
				Room Room = Dungeon.Level.GetRandomRoom();

				if (Room is ConnectionRoom || Room is BossEntranceRoom || Room is NpcSaveRoom) {
					continue;
				} 

				for (int J = 0; J < 32; J++) {
					Point Point = Room.GetRandomFreeCell();

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

		public Teleporter() {
			_Init();
		}
	}
}
