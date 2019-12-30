namespace BurningKnight.entity.creature.npc.dungeon {
	public class DungeonShopNpc : ShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			Remove = false;
		}
	}
}