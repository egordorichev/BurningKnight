using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class ShopKeeper : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AnimationComponent("shopkeeper"));
		}
	}
}