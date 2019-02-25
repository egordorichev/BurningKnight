using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.entity.item.active {
	public class SharpBlade : ActiveItem {
		public override Void Use() {
			if (Delay > 0) {
				return;
			} 

			base.Use();
			Player.Instance.ModifyHp(-1, null);
		}
	}
}
