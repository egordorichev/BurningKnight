using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.entity.item {
	public class SingleChoiceStand : ItemStand {
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			Subscribe<ItemTakenEvent>();
		}

		protected override string GetSprite() {
			return "single_stand";
		}

		protected override bool CanInteract(Entity e) {
			return Item != null && base.CanInteract(e);
		}

		protected virtual void RemoveStands() {
			var rm = GetComponent<RoomComponent>().Room;

			if (rm == null) {
				return;
			}

			GlobalSave.Put("item_stolen", true);
			var it = rm.Tagged[Tags.Item].ToArray(); // Copy it to prevent exceptions while modifying it
				
			foreach (var s in it) {
				if (s is SingleChoiceStand ist) {
					if (s is HealChoiceStand) {
						AnimationUtil.Poof(ist.Center);
						ist.Done = true;
					} else if (ist.Item != null) {
						var i = ist.Item;
						ist.SetItem(null, this);
						i.Done = true;
						AnimationUtil.Poof(ist.Center);
					}
				}
			}
					
			Camera.Instance.Shake(10);
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent ite && !(ite.Who is SingleChoiceStand || !(ite.Stand is SingleChoiceStand))) {
				var rm = GetComponent<RoomComponent>().Room;

				if (rm == null) {
					return false;
				}
				
				if (ite.Stand != this && ite.Stand.GetComponent<RoomComponent>().Room == rm) {
					RemoveStands();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}