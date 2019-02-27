using BurningKnight.entity.level.features;

namespace BurningKnight.entity.level.rooms.special {
	public class LockedRoomDef : SpecialRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.LOCKED);
		}
	}
}