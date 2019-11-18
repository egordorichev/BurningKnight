using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.entity.item.stand {
	public class GrannyStand : ItemStand {
		public override void Init() {
			base.Init();
			Subscribe<ItemTakenEvent>();
		}

		protected override string GetSprite() {
			return "granny_stand";
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent ite && !(ite.Who is GrannyStand || !(ite.Stand is GrannyStand))) {
				var rm = GetComponent<RoomComponent>().Room;
				
				if (ite.Stand != this && ite.Stand.GetComponent<RoomComponent>().Room == rm) {
					var it = rm.Tagged[Tags.Item].ToArray(); // Copy it to prevent exceptions while modifying it
				
					foreach (var s in it) {
						if (s is SingleChoiceStand ist && ist.Item != null) {
							var i = ist.Item;
							
							ist.SetItem(null, this);
							i.Done = true;
							ist.Done = true;
							AnimationUtil.Poof(ist.Center);
						}
					}
					
					Camera.Instance.Shake(10);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}