using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class PlaceDecoyUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var decoy = new Decoy();
			entity.Area.Add(decoy);
			decoy.BottomCenter = entity.BottomCenter;

			entity.GetComponent<BuffsComponent>().Add(new InvisibleBuff {
				Infinite = true
			});
			
			decoy.OnDeath += () => {
				entity.GetComponent<BuffsComponent>().Remove<InvisibleBuff>();
			};
		}
	}
}