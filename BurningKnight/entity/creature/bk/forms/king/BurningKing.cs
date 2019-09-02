using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class BurningKing : Boss {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new SliceComponent("items", "missing"));
			GetComponent<HealthComponent>().InitMaxHealth = 300;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height));
		}

		public override void SelectAttack() {
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(attacks.Next());
		}
		
		private SimpleAttackRegistry<BurningKing> attacks = new SimpleAttackRegistry<BurningKing>(new [] {
			new BossPattern<BurningKing>(typeof(JumpAttack))
		});
	}
}