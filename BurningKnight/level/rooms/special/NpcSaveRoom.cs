using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class NpcSaveRoom : SpecialRoom {
		public override int GetMaxConnections(Connection Side) {
			return Side == Connection.All ? 1 : 0;
		}

		public override int GetMinConnections(Connection Side) {
			return Side == Connection.All ? 1 : 0;
		}

		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && !(R is NpcKeyRoom);
		}

		public override void Paint(Level level) {
			ShopNpc npc = null;

			if (GlobalSave.IsFalse(ShopNpc.HatTrader)) {
				npc = new HatTrader();
			} else if (GlobalSave.IsFalse(ShopNpc.WeaponTrader)) {
				npc = new WeaponTrader();
			} else if (GlobalSave.IsFalse(ShopNpc.AccessoryTrader)) {
				npc = new AccessoryTrader();
			} else {
				npc = new ActiveTrader();
			}

			level.Area.Add(npc);
			npc.Center = GetTileCenter() * 16 + new Vector2(8);
		}

		public static bool ShouldBeAdded() {
			return GlobalSave.IsFalse(ShopNpc.AccessoryTrader) || GlobalSave.IsFalse(ShopNpc.ActiveTrader) ||
			       GlobalSave.IsFalse(ShopNpc.WeaponTrader) || GlobalSave.IsFalse(ShopNpc.HatTrader);
		}
	}
}