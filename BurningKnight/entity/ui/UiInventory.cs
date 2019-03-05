using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.ui {
	public class UiInventory : UiEntity {
		private Player player;
		
		public UiInventory(Player player) {
			this.player = player;
		}

		public override void Render() {
			
		}
	}
}