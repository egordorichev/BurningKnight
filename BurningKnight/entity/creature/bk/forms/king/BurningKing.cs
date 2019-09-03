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

			Width = 17;
			Height = 22;

			AddComponent(new ZComponent());
			AddComponent(new ZAnimationComponent("king"));
			AddComponent(new OrbitGiverComponent());
			
			GetComponent<HealthComponent>().InitMaxHealth = 300;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, center : true) {
				KnockbackModifier = 0.05f
			});

			var b = GetComponent<RectBodyComponent>().Body;
				
			b.LinearDamping = 0.5f;
			b.Restitution = 1;
			b.Friction = 0;
				
			AddComponent(new AimComponent(AimComponent.AimType.Target));
			
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
				// typeof(DashAttack)
				// typeof(JumpAttack)
				typeof(BladeAttack)
				
			)
		});
	}
}