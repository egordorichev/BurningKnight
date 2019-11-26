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
using Lens.util.tween;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.mob.prefabs {
	public class Slime : Mob {
		protected virtual float GetJumpDelay() {
			return 1;
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
				
				delay = Rnd.Float(0.5f, 1.2f) + Self.GetJumpDelay();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!tweened && T >= delay) {
					tweened = true;

					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("mob_slime_jump", 2);
				
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

		protected float JumpForce = 120;
		protected float ZVelocity = 5;

		public class JumpState : SmartState<Slime> {
			public override void Init() {
				base.Init();

				Self.OnJump();
				var a = Self.Target == null ? Rnd.AnglePI() : Self.AngleTo(Self.Target) + Rnd.Float(-0.1f, 0.1f);
				var force = Rnd.Float(20f) + Self.JumpForce;
				
				Self.GetComponent<RectBodyComponent>().Velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);

				Self.GetComponent<ZComponent>().ZVelocity = Self.ZVelocity;
			}

			public override void Destroy() {
				base.Destroy();
				
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.OnLand();

				var anim = Self.GetComponent<ZAnimationComponent>();

				anim.Scale.X = 2f;
				anim.Scale.Y = 0.3f;
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("mob_slime_land", 2);
				
				Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.3f);
				Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f);
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
			return "slime_dead";
		}

		protected override string GetHurtSfx() {
			return "slime_hurt";
		}
	}
}