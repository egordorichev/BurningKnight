using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.math;
using Lens.util.timer;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob {
	public class Dummy : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new RectBodyComponent(4, 2, 8, 14, BodyType.Static));
			AddComponent(new DialogComponent());
			AddAnimation("dummy");
			
			SetMaxHp(1);
			RemoveTag(Tags.MustBeKilled);
			
			Become<IdleState>();

			var health = GetComponent<HealthComponent>();

			health.InvincibilityTimerMax = 0;
			health.RenderInvt = false;

			TouchDamage = 0;
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev && ev.Amount < 0) {
				Become<HurtState>();
				GraphicsComponent.Flipped = ev.From.CenterX > CenterX;

				if (Run.Depth < 1 && Random.Chance(30)) {
					var dialog = GetComponent<DialogComponent>();

					if (dialog.Current == null) {
						dialog.StartAndClose($"npc_hurt_{Random.Int(3)}", 2);
					}
				}
				
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
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<MobAnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>(true);
				}
			}
		}
		#endregion
	}
}