using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Slime : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZComponent());
			AddComponent(new ZAnimationComponent("slime"));
			SetMaxHp(2);

			if (Random.Chance()) {
				Become<JumpState>();
			} else {
				Become<IdleState>();
			}
			
			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}
		
		#region Slime States
		public class IdleState : MobState<Slime> {
			private float delay;
			private bool tweened;
			
			public override void Init() {
				base.Init();
				// fixme: should not go through walls and destroyable
				
				delay = Random.Float(0.9f, 1.2f);
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!tweened && T >= delay) {
					tweened = true;
					
				
					var anim = Self.GetComponent<ZAnimationComponent>();
				
					Tween.To(2f, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
					Tween.To(0.3f, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f).OnEnd = () => {
						Tween.To(0.5f, anim.Scale.X, x => anim.Scale.X = x, 0.3f);

						Tween.To(2f, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f).OnEnd = () => {
							Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
							Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f);
						};

						Become<JumpState>();
					};
				}
			}
		}

		public class JumpState : MobState<Slime> {
			private Vector2 velocity;
			private float zVelocity;
			
			public override void Init() {
				base.Init();

				var a = Self.Target == null ? Random.AnglePI() : Self.AngleTo(Self.Target) + Random.Float(-0.1f, 0.1f);
				var force = Random.Float(20f) + 120f;
				
				velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				
				zVelocity = 5;
			}

			public override void Destroy() {
				base.Destroy();

				var anim = Self.GetComponent<ZAnimationComponent>();

				anim.Scale.X = 2f;
				anim.Scale.Y = 0.3f;

				Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.3f);
				Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var component = Self.GetComponent<ZComponent>();
				component.Z += zVelocity * dt * 20;

				if (component.Z >= 4f) {
					Self.Depth = Layers.FlyingMob;
					Self.TouchDamage = 0;
				} else {
					Self.Depth = Layers.Creature;
					Self.TouchDamage = 1;
				}

				zVelocity -= dt * 10;
				
				if (component.Z <= 0) {
					component.Z = 0;
					Become<IdleState>();
				}
			}
		}
		#endregion

		public override bool InAir() {
			return (GetComponent<StateComponent>().StateInstance is JumpState);
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Chasm) {
				return !InAir();
			}
			
			return base.ShouldCollide(entity);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev && ev.Amount < 0 && InAir() && GetComponent<ZComponent>().Z > 4f) {
				return true;
			}
			
			return base.HandleEvent(e);
		}

		public override bool ShouldCollideWithDestroyableInAir() {
			return true;
		}
	}
}