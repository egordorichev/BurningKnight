using BurningKnight.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class HatTrader : ShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 13;
			Height = 20;
			
			AddComponent(new AnimationComponent("hat_trader"));

			var b = new RectBodyComponent(2, 6, Width - 4, Height - 6);
			AddComponent(b);
			b.KnockbackModifier = 0;
		}

		protected override string GetDialog() {
			return $"hattrader_{Rnd.Int(3)}";
		}

		public override string GetId() {
			return HatTrader;
		}
	}
}