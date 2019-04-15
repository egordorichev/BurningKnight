using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new AnimationComponent("crawler"));
			SetMaxHp(2);
			Become<IdleState>();

			Width = 24;
			Height = 14;

			var body = new RectBodyComponent(4, 3, 16, 9);
			AddComponent(body);
			body.Body.LinearDamping = 5;
			body.KnockbackModifier = 0;
			
			// todo: lock to a wall
		}
		
		#region Crawler States
		public class IdleState : MobState<WallCrawler> {
			
		}
		#endregion

		public override bool SpawnsNearWall() {
			return true;
		}
	}
}