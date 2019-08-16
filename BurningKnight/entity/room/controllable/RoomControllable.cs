using BurningKnight.save;

namespace BurningKnight.entity.room.controllable {
	public class RoomControllable : SaveableEntity {
		protected bool On;
		
		public virtual void TurnOn() {
			On = true;
		}

		public virtual void TurnOff() {
			On = false;
		}
		
		public void Toggle() {
			if (On) {
				TurnOff();
			} else {
				TurnOn();
			}
		}
	}
}