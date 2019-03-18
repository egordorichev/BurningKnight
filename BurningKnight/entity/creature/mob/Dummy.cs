using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob {
	public class Dummy : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new RectBodyComponent(4, 2, 8, 14, BodyType.Static));
			AddAnimation("dummy");
			SetMaxHp(10);
			
			Become<IdleState>();

			var health = GetComponent<HealthComponent>();

			health.InvincibilityTimerMax = 0;
			health.RenderInvt = false;
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev && ev.Amount < 0) {
				Become<HurtState>();
				GraphicsComponent.Flipped = ev.From.CenterX > CenterX;
				
				return true;
			}
			
			return base.HandleEvent(e);
		}

		#region Dummy States
		public class IdleState : EntityState {
			
		}

		public class HurtState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>(true);
				}
			}
		}
		#endregion
	}
}