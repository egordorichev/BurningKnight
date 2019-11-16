using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.boss {
	public class OldKing : Boss {
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 27;
			
			AddComponent(new AnimationComponent("old_king"));
			AddComponent(new RectBodyComponent(3, 10, 14, 17));
			
			Become<IdleState>();
			
			SetMaxHp(80);
		}
		
		#region Old King States
		public class IdleState : SmartState<OldKing> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					T = 0;
					return;
				}

				if (T >= 5f) {
					Become<JumpState>();
				}
			}
		}
		
		public class JumpState : SmartState<OldKing> {
			public override void Init() {
				
			}

			public override void Update(float dt) {
				base.Update(dt);
			}
		}
		
		public class UpState : SmartState<OldKing> {
			
		}
		
		public class DownState : SmartState<OldKing> {
			
		}
		
		public class LandState : SmartState<OldKing> {
			
		}
		
		public class RunState : SmartState<OldKing> {
			
		}
		#endregion
	}
}