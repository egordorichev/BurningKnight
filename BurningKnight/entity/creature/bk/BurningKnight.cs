using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.ui;
using Lens;
using Lens.entity.component.logic;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private HealthBar healthBar;
	
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.BurningKnight);

			Width = 42;
			Height = 42;
			TouchDamage = 0;

			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			// FIXME: TMP sprite and size, obv
			AddComponent(new AnimationComponent("burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = 1024;
			
			GetComponent<StateComponent>().Become<IdleState>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (healthBar == null) {
				healthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(healthBar);
			}
		}

		#region Burning Knight States

		#endregion
	}
}