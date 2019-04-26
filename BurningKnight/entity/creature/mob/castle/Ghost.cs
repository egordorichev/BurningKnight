using System;
using BurningKnight.entity.component;
using BurningKnight.level;
using Lens.entity;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Ghost : Mob {
		public const float TargetRadius = 128f;
		public const byte Alpha = 50;
		
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("ghost");
			SetMaxHp(2);
			
			Flying = true;
			Depth = Layers.FlyingMob;
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 3;
			
			GetComponent<AnimationComponent>().Tint.A = Alpha;
		}

		private bool rage;
		
		#region Ghost States
		public class IdleState : MobState<Ghost> {
			public override void Init() {
				base.Init();

				var color = Self.GetComponent<AnimationComponent>().Tint;
				Tween.To(Alpha, color.A, x => Self.GetComponent<AnimationComponent>().Tint.A = (byte) x, 0.3f);
			}

			public override void Destroy() {
				base.Destroy();	
				
				var color = Self.GetComponent<AnimationComponent>().Tint;
				Tween.To(255f, color.A, x => Self.GetComponent<AnimationComponent>().Tint.A = (byte) x, 0.3f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.Target.DistanceSquaredTo(Self) < TargetRadius) {
					Become<ChaseState>();
				}

				Self.CheckRage();
			}
		}

		public class ChaseState : MobState<Ghost> {
			private bool rage;

			public override void Init() {
				base.Init();
				rage = Self.rage;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					return;
				}
				
				// Fixme: this target stuff is bad for multiplayer, it will chase only random player, not closest
				float d = Self.Target.DistanceSquaredTo(Self);

				if (!rage) {
					Self.CheckRage();

					if (Self.rage) {
						rage = true;
					} else if (d > TargetRadius + 10f) {
						Become<IdleState>();
						return;
					}
				}

				float a = Self.AngleTo(Self.Target);
				float force = (rage ? 200f : 100f) * dt;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		#endregion
		
		protected void CheckRage() {
			if (Target == null) {
				return;
			}
			
			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}
			
			foreach (var m in room.Tagged[Tags.MustBeKilled]) {
				if (!(m is Ghost)) {
					return;
				}
			}
				
			// RAAAAAAGE MOOOOOOOOOOOODE
			rage = true;
			
			Become<ChaseState>();
			GetComponent<HealthComponent>().InitMaxHealth = 4;
		}

		public override bool ShouldCollide(Entity entity) {
			return base.ShouldCollide(entity) && !(entity is Level);
		}

		public override float GetSpawnChance() {
			return 0.6f;
		}
	}
}