using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.util;
using Lens.entity;

namespace BurningKnight.entity.item {
	public class SingleChoiceStand : ItemStand {
		public override void Init() {
			base.Init();
			Subscribe<ItemTakenEvent>();
		}

		protected override string GetSprite() {
			return "single_stand";
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent ite && !(ite.Who is SingleChoiceStand)) {
				var rm = GetComponent<RoomComponent>().Room;
				
				if (ite.Stand != this && ite.Stand.GetComponent<RoomComponent>().Room == rm) {
					var it = rm.Tagged[Tags.Item].ToArray(); // Copy it to prevent execptions while modifying it
				
					foreach (var s in it) {
						if (s is ItemStand ist && ist.Item != null) {
							var i = ist.Item;
							ist.SetItem(null, this);
							i.Done = true;
							AnimationUtil.Poof(ist.Center);
						}
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}