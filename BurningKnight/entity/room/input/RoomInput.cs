using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.editor;
using Lens.entity;

namespace BurningKnight.entity.room.input {
	public class RoomInput : SaveableEntity, PlaceableEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RoomComponent());
			AddComponent(new SupportableComponent());
		}

		public bool On { get; protected set; }
		
		public virtual void TurnOn() {
			if (On) {
				return;
			}
			
			On = true;
			UpdateState();
			SendEvents();
		}
		
		public virtual void TurnOff() {
			if (!On) {
				return;
			}
			
			On = false;
			UpdateState();
			SendEvents();
		}

		protected virtual void UpdateState() {
			
		}

		protected void SendEvents() {
			var e = new ChangedEvent {
				Input = this
			};
			
			HandleEvent(e);
			GetComponent<RoomComponent>().Room?.HandleInputChange(e);
		}

		public void Toggle() {
			if (On) {
				TurnOff();
			} else {
				TurnOn();
			}
		}
		
		public class ChangedEvent : Event {
			public RoomInput Input;
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
				room.Inputs.Add(this);
			}
		}

		protected void RemoveFromRoom() {
			GetComponent<RoomComponent>().Room?.Inputs.Remove(this);
		}
	}
}