using BurningKnight.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class ActiveTrader : ShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 17;
			Height = 22;
			
			AddComponent(new AnimationComponent("active_trader"));

			var b = new RectBodyComponent(2, 6, Width - 4, Height - 6);
			AddComponent(b);
			b.KnockbackModifier = 0;
		}

		protected override string GetDialog() {
			return $"activetrader_{Random.Int(3)}";
		}

		public override string GetId() {
			return ActiveTrader;
		}
	}
}