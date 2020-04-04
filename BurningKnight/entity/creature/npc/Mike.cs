using BurningKnight.entity.component;
using BurningKnight.save;

namespace BurningKnight.entity.creature.npc {
	public class Mike : ShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 11;
			Height = 13;
			
			AddComponent(new AnimationComponent("mike"));
			// GetComponent<DialogComponent>().Dialog.Voice = 15;
		}

		protected override string GetDialog() {
			return "mike_1";
		}

		public override string GetId() {
			return Mike;
		}
	}
}