using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.active {
	public class KillerItem : ActiveItem {
		public override void Use() {
			base.Use();
			Player.Instance.ModifyHp(-Player.Instance.GetHpMax(), null, true);
			Player.Instance.SetHpMax(0);
			Player.Instance.Die();
			Player.Instance.PlaySfx("head_explode");
		}
	}
}