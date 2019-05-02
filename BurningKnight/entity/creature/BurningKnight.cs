using BurningKnight.entity.creature.mob;
using BurningKnight.ui;
using Lens;

namespace BurningKnight.entity.creature {
	public class BurningKnight : Mob {
		private HealthBar healthBar;
		
		public override void AddComponents() {
			base.AddComponents();
			
			RemoveTag(Tags.LevelSave);
			AddTag(Tags.PlayerSave);
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("burning_knight");
			SetMaxHp(10000);
			
			Become<IdleState>();
		}

		public override void Init() {
			base.Init();
			
			healthBar = new HealthBar(this);
			Engine.Instance.State.Ui.Add(healthBar);
		}

		#region Burning Knight States
		public class IdleState : MobState<BurningKnight> {
			
		}
		#endregion
	}
}