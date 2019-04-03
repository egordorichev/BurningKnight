using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature {
		public override void AddComponents() {
			base.AddComponents();
			
			// Graphics
			// AddComponent(new LightComponent(this, 128f, new Color(1, 1, 1, 1f)));
			AddComponent(new PlayerGraphicsComponent {
				Offset = new Vector2(0, -4)
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
			AddComponent(new RectBodyComponent(2, 0, 12, 12));
			AddComponent(new InteractorComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;
			
			// Simple inventory simulation
			var inventory = GetComponent<InventoryComponent>();
			inventory.Pickup(ItemRegistry.Create("gun", Area));

			GetComponent<HealthComponent>().MaxHealth = 1;
			
			AddDrops(new SingleDrop("heart", 1f), new SingleDrop("heart", 1f), new SingleDrop("heart", 1f));
		}

		public override void PostInit() {
			base.PostInit();

			// todo: pick the entrance one
			var room = Area.Tags[Tags.Room][0];
			Center = room.Center;

			ItemStand stand;
			
			Area.Add(stand = new ItemStand {
				Center = room.Center - new Vector2(0, 16)
			});
			
			stand.SetItem(ItemRegistry.Create("heart", Area), this);
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
				body.Position += body.Velocity * dt;
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Player) && base.ShouldCollide(entity);
		}

		protected override bool HasNoHealth(HealthModifiedEvent e) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == (e.Default ? 0 : -e.Amount);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
		}
	}
}