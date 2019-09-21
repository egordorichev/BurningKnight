using BurningKnight.entity.component;
using BurningKnight.save;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class AccessoryTrader : ShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 12;
			Height = 15;
			
			AddComponent(new AnimationComponent("accessory_trader"));

			var b = new RectBodyComponent(2, 6, Width - 4, Height - 6);
			AddComponent(b);
			b.KnockbackModifier = 0;
		}

		protected override string GetDialog() {
			return $"accessorytrader_{Random.Int(3)}";
		}
	}
}