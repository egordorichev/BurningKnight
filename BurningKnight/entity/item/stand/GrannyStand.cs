using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;
using Lens.util.camera;

namespace BurningKnight.entity.item.stand {
	public class GrannyStand : ItemStand {
		protected override string GetSprite() {
			return "granny_stand";
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
		
			var rm = GetComponent<RoomComponent>().Room;
			var it = rm.Tagged[Tags.Item].ToArray(); // Copy it to prevent exceptions while modifying it
			
			foreach (var s in it) {
				if (s != this && s is GrannyStand ist && ist.Item != null) {
					ist.Item.Done = true;
					ist.Done = true;
					AnimationUtil.Poof(ist.Center);
				}
			}
			
			Done = true;
			Camera.Instance.Shake(10);
			Achievements.Unlock("bk:grannys_gift");
		}
	}
}