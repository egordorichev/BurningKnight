using System;
using BurningKnight.entity.component;
using BurningKnight.level;
using BurningKnight.level.entities;
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

			var body = new SensorBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 3;
			
			GetComponent<MobAnimationComponent>().Tint.A = Alpha;
		}

		private bool rage;
		
		#region Ghost States
		public class IdleState : SmartState<Ghost> {
			public override void Init() {
				base.Init();

				var color = Self.GetComponent<MobAnimationComponent>().Tint;
				Tween.To(Alpha, color.A, x => Self.GetComponent<MobAnimationComponent>().Tint.A = (byte) x, 0.3f);
			}

			public override void Destroy() {
				base.Destroy();	
				
				var color = Self.GetComponent<MobAnimationComponent>().Tint;
				Tween.To(255f, color.A, x => Self.GetComponent<MobAnimationComponent>().Tint.A = (byte) x, 0.3f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.Target.DistanceTo(Self) < TargetRadius) {
					Become<ChaseState>();
				}

				Self.CheckRage();
			}
		}

		public class ChaseState : SmartState<Ghost> {
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
				float d = Self.Target.DistanceTo(Self);

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

				Self.GetComponent<SensorBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				Self.PushFromOtherEnemies(dt);
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
			if (entity is Prop) {
				return false;
			}
			
			return base.ShouldCollide(entity) && !(entity is Level);
		}

		public override float GetSpawnChance() {
			return 0.6f;
		}
	}
}