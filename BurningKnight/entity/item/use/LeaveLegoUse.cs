using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class LeaveLegoUse : ItemUse {
		private float t;
		
		public override void Update(Entity entity, Item item, float dt) {
			base.Update(entity, item, dt);

			var r = entity.GetComponent<RoomComponent>().Room;

			if (r == null || r.Tagged[Tags.MustBeKilled].Count == 0) {
				t = 0;
				return;
			}

			t += dt;

			if (t >= 5f) {
				t = 0;
				var lego = new Lego();
				entity.Area.Add(lego);
				lego.BottomCenter = entity.BottomCenter;
				AnimationUtil.Ash(lego.Center);
			}
		}
	}
}