using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.editor;

namespace BurningKnight.entity.room.controllable {
	public class RoomControllable : SaveableEntity, PlaceableEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RoomComponent());
			AddComponent(new SupportableComponent());
		}
		
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

		private bool added;

		public override void Update(float dt) {
			base.Update(dt);

			if (!added) {
				var room = GetComponent<RoomComponent>().Room;

				if (room == null) {
					return;
				}
				
				added = true;
				room.Controllable.Add(this);
			}
		}

		protected void RemoveFromRoom() {
			GetComponent<RoomComponent>().Room?.Controllable.Remove(this);
		}
	}
}