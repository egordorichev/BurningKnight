using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.pet {
	public class LilBoo : Pet {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 6;
			Height = 8;
			
			AddComponent(new BooGraphicsComponent("ghost_pet"));

			var b = new RectBodyComponent(0, 0, Width, Height) {
				KnockbackModifier = 0.2f
			};
			
			AddComponent(b);
			b.Body.LinearDamping = 3;
			
			
			Become<IdleState>();
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Player) {
				return true;
			}

			if (GetComponent<StateComponent>().StateInstance is WanderState) {
				return entity is DestroyableLevel || entity is Level;
			}

			return false;
		}

		#region Boo states
		private class IdleState : SmartState<LilBoo> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Owner);

				if (d > 48) {
					Self.Become<FollowState>();
				}
			}
		}

		private class FollowState : SmartState<LilBoo> {
			public override void Update(float dt) {
				base.Update(dt);

				var dx = Self.DxTo(Self.Owner);
				var dy = Self.DyTo(Self.Owner);
				var d = MathUtils.Distance(dx, dy);
				
				if (d > 256) {
					AnimationUtil.Poof(Self.Center);
					Self.Center = Self.Owner.Center + Random.Offset(24);
					AnimationUtil.Poof(Self.Center);
					
					Self.Become<HappyState>();

					return;
				}

				if (d < 32) {
					Self.Become<HappyState>();
					return;
				}

				var body = Self.GetComponent<RectBodyComponent>();
				var s = dt * 20;
				
				body.Velocity += new Vector2(dx / d * s, dy / d * s);
			}
		}

		private class EmotionState : SmartState<LilBoo> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T > 1f) {
					Self.Become<IdleState>();
				}
			}
		}
		
		private class HappyState : EmotionState {
			
		}

		private class WanderState : SmartState<LilBoo> {
			
		}
		#endregion
	}
}