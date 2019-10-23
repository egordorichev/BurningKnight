using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.ui.dialog;
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

				foreach (var n in r.Tagged[Tags.Npc]) {
					if (n is OldMan) {
						n.GetComponent<DialogComponent>().Start("old_man_5");
						n.RemoveComponent<CloseDialogComponent>();

						break;
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}