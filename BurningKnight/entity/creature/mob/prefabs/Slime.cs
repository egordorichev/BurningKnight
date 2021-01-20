using System;
using System.Numerics;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.mob.prefabs {
	public class Slime : Mob {
		private bool first;
		private float delay;
		
		private static readonly Color color = ColorUtils.FromHex("#5ac54f");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target != null) {
				delay = Rnd.Float(0, 2f);
			}
		}

		protected virtual float GetJumpDelay() {
			return 1;
		}

		protected virtual float GetJumpAngle() {
			return Target == null ? Rnd.AnglePI() : AngleTo(Target) + Rnd.Float(-0.1f, 0.1f);
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZComponent());
			Become<IdleState>();
			
			AddDrops(new SingleDrop("bk:slime", 0.01f));
		}
		
		#region Slime States
		public class IdleState : SmartState<Slime> {
			private float delay;
			private bool tweened;
			
			public override void Init() {
				base.Init();
				
				delay = Self.first ? Rnd.Float(1f) : Self.GetJumpDelay();
				Self.first = false;

				// To avoid exceptions when loading old save due to component not being there

				if (Self.TryGetComponent<RectBodyComponent>(out var c)) {
					c.Velocity = Vector2.Zero;
				}
			}

			public override void Update(float dt) {
				if (Self.delay > 0) {
					Self.delay -= dt;
					return;
				}
				
				base.Update(dt);

				if (!tweened && T >= delay) {
					tweened = true;

					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("mob_slime_jump", 2);
					
					Self.AnimateJump(() => {
						Become<JumpState>();
					});
				}
			}
		}

		protected float JumpForce = 120;
		protected float ZVelocity = 5;
		
		protected virtual void AnimateJump(Action callback) {
			var anim = GetComponent<ZAnimationComponent>();
				
			Tween.To(2f, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
			Tween.To(0.3f, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(0.5f, anim.Scale.X, x => anim.Scale.X = x, 0.3f);

				Tween.To(2f, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f).OnEnd = () => {
					Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
					Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f);
				};

				callback();
			};
		}

		protected virtual void AnimateLand() {
			var anim = GetComponent<ZAnimationComponent>();

			anim.Scale.X = 2f;
			anim.Scale.Y = 0.3f;
			Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.3f);
			Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f);
		}
		
		public class JumpState : SmartState<Slime> {
			public bool InAir;
			
			public override void Init() {
				base.Init();

				InAir = true;
				Self.OnJump();
				var a = Self.GetJumpAngle();
				var force = Rnd.Float(20f) + Self.JumpForce;
				
				Self.GetComponent<RectBodyComponent>().Velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);

				Self.GetComponent<ZComponent>().ZVelocity = Self.ZVelocity;
			}

			public override void Destroy() {
				base.Destroy();
				
				Self.AnimateLand();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.OnLand();
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("mob_slime_land", 2);
			}
			
			public override void Update(float dt) {
				base.Update(dt);

				var component = Self.GetComponent<ZComponent>();

				if (component.Z >= 4f) {
					Self.Depth = Layers.FlyingMob;
					Self.TouchDamage = 0;
				} else {
					Self.Depth = Layers.Creature;
					Self.TouchDamage = 1;
				}
				
				if (T >= 0.1f && component.Z <= 0) {
					component.Z = 0;
					InAir = false;
					Become<IdleState>();
				}
			}
		}
		#endregion

		public override bool InAir() {
			return (GetComponent<StateComponent>().StateInstance is JumpState j && j.InAir);
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

		public override bool IgnoresProjectiles() {
			return InAir();
		}

		protected virtual void OnJump() {
			
		}

		protected virtual void OnLand() {
			if (Target == null) {
				return;
			}
			
			Area.Add(new SplashFx {
				Position = Center,
				Color = ColorUtils.Mod(GetBloodColor())
			});
		}

		protected override string GetDeadSfx() {
			return "mob_slime_death";
		}

		protected override string GetHurtSfx() {
			return "slime_hurt";
		}
	}
}