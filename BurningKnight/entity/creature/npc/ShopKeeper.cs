using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.npc {
	public class ShopKeeper : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AnimationComponent("shopkeeper"));

			var h = GetComponent<HealthComponent>();

			h.InitMaxHealth = 10;
			h.Unhittable = false;
			
			AddComponent(new RectBodyComponent(4, 2, 10, 14));
		}
	}
}