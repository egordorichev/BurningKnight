using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.rooms;
using BurningKnight.physics;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature {
		public override void AddComponents() {
			base.AddComponents();
			
			Height = 11;
			
			// Graphics
			// AddComponent(new LightComponent(this, 128f, new Color(1, 1, 1, 1f)));
			AddComponent(new PlayerGraphicsComponent {
				Offset = new Vector2(0, -5)
			});
			
			// Inventory
			AddComponent(new InventoryComponent());
			AddComponent(new LampComponent());
			AddComponent(new ActiveItemComponent());
			AddComponent(new ActiveWeaponComponent());
			AddComponent(new WeaponComponent());
			AddComponent(new ConsumablesComponent());
						
			// Stats
			AddComponent(new HeartsComponent());
			
			// Collisions
			AddComponent(new RectBodyComponent(4, 3, 8, 9));
			AddComponent(new InteractorComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();

			if (Engine.Version.Dev) {
				var health = GetComponent<HealthComponent>();

				health.Unhittable = true;
				health.MaxHealth = HeartsComponent.Cap;
				health.SetHealth(health.MaxHealth, this);
			}
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;
			
			// Simple inventory simulation
			var inventory = GetComponent<InventoryComponent>();
			inventory.Pickup(Items.CreateAndAdd("bk:sword", Area));

			GetComponent<HealthComponent>().MaxHealth = 1;
		}

		public override void PostInit() {
			base.PostInit();

			foreach (var r in Area.Tags[Tags.Room]) {
				var rm = (Room) r;
				
				if (rm.Type == RoomType.Entrance) {
					Center = r.Center;
					rm.Discover();

					return;
				}
			}
			
			foreach (var r in Area.Tags[Tags.Room]) {
				var rm = (Room) r;
				
				if (rm.Type == RoomType.Exit) {
					Center = new Vector2(rm.CenterX, rm.Bottom - 1.4f * 16);
					rm.Discover();

					return;
				}
			}
		}
		
		#region Player States
		public class IdleState : EntityState {
			
		}
		
		public class RunState : EntityState {
			
		}
		
		public class RollState : EntityState {
			private const float RollTime = 0.39f;
			private const float RollForce = 400f;
			
			private Vector2 direction;
			
			public override void Init() {
				base.Init();
				
				Self.GetComponent<HealthComponent>().Unhittable = true;

				var body = Self.GetComponent<RectBodyComponent>();
				var angle = body.Acceleration.LengthSquared() > 0.1f 
					?	body.Acceleration.ToAngle() 
					: (Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition) - Self.Center).ToAngle();

				direction = new Vector2((float) Math.Cos(angle) * RollForce, (float) Math.Sin(angle) * RollForce);
			}

			public override void Destroy() {
				base.Destroy();
				
				Self.GetComponent<HealthComponent>().Unhittable = false;
				Self.GetComponent<RectBodyComponent>().Acceleration = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= RollTime) {
					Become<IdleState>();
					return;
				}
				
				var body = Self.GetComponent<RectBodyComponent>();
				body.Velocity = direction * (RollTime - T);
				body.Position += body.Velocity * dt * 0.5f;
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Player) && base.ShouldCollide(entity);
		}

		public override bool InAir() {
			return base.InAir() || GetComponent<StateComponent>().StateInstance is RollState;
		}

		public override bool HasNoHealth(HealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == (e == null ? 0 : (e.Default ? 0 : -e.Amount));
		}

		public override bool HandleEvent(Event e) {
			if (e is LostSupportEvent) {
				GetComponent<HealthComponent>().ModifyHealth(-1, this);
				return true;
			} else if (e is RoomChangedEvent c) {
				c.New.Discover();
			}
			
			return base.HandleEvent(e);
		}
	}
}