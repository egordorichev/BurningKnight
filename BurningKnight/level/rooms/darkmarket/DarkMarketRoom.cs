using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.level.rooms.special;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.darkmarket {
	public class DarkMarketRoom : SpecialRoom {
		public override void Paint(Level level) {
			Roger.Place(new Vector2(Left + 3.5f, Top + 3.5f) * 16, level.Area);
			Boxy.Place(new Vector2(Right - 2.5f, Top + 3.5f) * 16, level.Area);
			
			Snek.Place(new Vector2(Left + 3.5f, Bottom - 3f) * 16, level.Area);
			Gobetta.Place(new Vector2(Right - 2.5f, Bottom - 3f) * 16, level.Area);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Head;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right || P.Y == Top) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}

		public override int GetMinWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 12;
		}

		public override int GetMaxWidth() {
			return 15;
		}

		public override int GetMaxHeight() {
			return 13;
		}
	}
}