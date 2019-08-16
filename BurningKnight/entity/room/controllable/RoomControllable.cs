using BurningKnight.save;
using BurningKnight.ui.editor;

namespace BurningKnight.entity.room.controllable {
	public class RoomControllable : SaveableEntity, PlaceableEntity {
		public bool On { get; private set; }
		
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