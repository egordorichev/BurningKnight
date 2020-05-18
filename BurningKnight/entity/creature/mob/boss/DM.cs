using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.boss {
	public class DM : Boss {
		public override void AddComponents() {
			base.AddComponents();

			Width = 14;
			Height = 17;
			
			AddComponent(new SensorBodyComponent(1, 1, 12, 16));

			var body = new RectBodyComponent(1, 16, 12, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.5f;
			body.Body.LinearDamping = 3;

			AddAnimation("dark_mage");
			SetMaxHp(1000);
		}
	}
}