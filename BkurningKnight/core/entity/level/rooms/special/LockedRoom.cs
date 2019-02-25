using BurningKnight.core.entity.level.features;

namespace BurningKnight.core.entity.level.rooms.special {
	public class LockedRoom : SpecialRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.LOCKED);
			}
		}
	}
}
