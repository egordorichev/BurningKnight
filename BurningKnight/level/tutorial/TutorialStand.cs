using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.level.tutorial {
	public class TutorialStand : ItemStand {
		private bool set;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (!set) {
				set = true;

				if (Item != null) {
					foreach (var c in GetComponent<RoomComponent>().Room.Controllable) {
						c.TurnOff();
					}
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent) {
				Camera.Instance.Shake(8);
				var r = GetComponent<RoomComponent>().Room;
				
				foreach (var c in r.Controllable) {
					c.TurnOn();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}