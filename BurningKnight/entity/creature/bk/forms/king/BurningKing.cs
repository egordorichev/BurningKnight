using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.item;
using Lens;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class BurningKing : Boss {
		private Item sword;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new SliceComponent("items", "missing"));
			GetComponent<HealthComponent>().InitMaxHealth = 300;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height));

			sword = Items.CreateAndAdd("bk:burning_king_sword", Area);

			if (sword != null) {
				sword.RemoveDroppedComponents();
				sword.AddComponent(new OwnerComponent(this));
			}
		}

		public override void Render() {
			base.Render();
			sword?.Renderer.Render(false, false, Engine.Delta, false);
		}

		protected override void RenderShadow() {
			base.RenderShadow();
			sword?.Renderer.Render(false, false, Engine.Delta, true);
		}

		public override void SelectAttack() {
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(attacks.Next());
		}
		
		private SimpleAttackRegistry<BurningKing> attacks = new SimpleAttackRegistry<BurningKing>(new [] {
			new BossPattern<BurningKing>(
				typeof(JumpAttack)
			)
		});
	}
}