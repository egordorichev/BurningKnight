using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.timer;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public bool Awoken;
		
		protected bool HasHealthbar = true;
		protected HealthBar HealthBar;

		public override void AddComponents() {
			base.AddComponents();
			GetComponent<BuffsComponent>().AddImmunity<CharmedBuff>();
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (HasHealthbar && HealthBar == null) {
				HealthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(HealthBar);
			}
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Awoken = false;
			} else {
				Awoken = true;
			}
			
			base.OnTargetChange(target);
		}

		public virtual void SelectAttack() {
			
		}
	}
}