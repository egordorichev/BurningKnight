using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.entity.item {
	public class SingleChoiceStand : ItemStand {
		public override void Init() {
			base.Init();
			Subscribe<ItemTakenEvent>();
		}

		protected override string GetSprite() {
			return "single_stand";
		}

		protected override bool CanInteract(Entity e) {
			return Item != null && base.CanInteract(e);
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent ite && !(ite.Who is SingleChoiceStand || !(ite.Stand is SingleChoiceStand))) {
				var rm = GetComponent<RoomComponent>().Room;
				
				if (ite.Stand != this && ite.Stand.GetComponent<RoomComponent>().Room == rm) {
					var it = rm.Tagged[Tags.Item].ToArray(); // Copy it to prevent exceptions while modifying it
				
					foreach (var s in it) {
						if (s is SingleChoiceStand ist && ist.Item != null) {
							var i = ist.Item;
							ist.SetItem(null, this);
							i.Done = true;
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