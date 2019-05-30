using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, DropModifier {
		public static string StartingLamp;
		public static string StartingWeapon;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Height = 11;
			
			// Graphics
			AddComponent(new LightComponent(this, 128f, new Color(0.3f, 0.4f, 0.3f, 0.2f)));
			
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
			AddComponent(new InteractorComponent {
				CanInteractCallback = e => !(GetComponent<StateComponent>().StateInstance is GotState || died)
			});
			
			// Other mechanics
			AddComponent(new OrbitGiverComponent());
			AddComponent(new FollowerComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;

			GetComponent<HealthComponent>().MaxHealth = 1;
		}

		public void FindSpawnPoint() {
			if (Run.StartedNew) {
				if (StartingWeapon == null) {
					StartingWeapon = Items.Generate(ItemPool.StartingWeapon, item => Item.Unlocked(item.Id));
				}

				if (StartingWeapon != null) {
					GetComponent<ActiveWeaponComponent>().Set(Items.CreateAndAdd(StartingWeapon, Area));
				}

				if (StartingLamp != null) {
					GetComponent<LampComponent>().Set(Items.CreateAndAdd(StartingLamp, Area));
				}
			}
			
			foreach (var c in Area.Tags[Tags.Checkpoint]) {
				Center = c.Center;
				Log.Debug("Teleported to spawn point");
				return;
			}

			foreach (var c in Area.Tags[Tags.Entrance]) {
				Center = c.Center + new Vector2(0, 4);
				Log.Debug("Teleported to entrance");
				return;
			}
			
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
			
			Log.Error("Did not find a spawn point!");
		}
		
		#region Player States
		public class IdleState : EntityState {
			
		}
		
		public class RunState : EntityState {
			
		}
		
		public class GotState : EntityState {
			private const float Delay = 1f;
			
			private bool tweened;
			public Item Item;
			public Action<Item> OnEnd;
			public Vector2 Scale;

			public override void Init() {
				base.Init();

				Tween.To(1, 0, x => {
					Scale.X = x;
					Scale.Y = x;
				}, 0.5f, Ease.BackOut);
			}

			public override void Destroy() {
				base.Destroy();

				if (OnEnd != null) {
					OnEnd(Item);
				} else {
					Self.GetComponent<InventoryComponent>().Add(Item);
					Item.Use(Self);	
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!tweened && T >= Delay) {
					tweened = true;
					
					Tween.To(0, 1, x => {
						Scale.X = x;
						Scale.Y = x;
					}, 0.3f, Ease.BackIn).OnEnd = Become<IdleState>;
				}
			}
		}
		
		public class RollState : EntityState {
			private const float RollTime = 0.39f;
			private const float RollForce = 400f;
			
			private float lastParticle = 0.05f;
			private Vector2 direction;
			private bool wasUnhittable;
			
			public override void Init() {
				base.Init();

				var hp = Self.GetComponent<HealthComponent>();

				wasUnhittable = hp.Unhittable;
				hp.Unhittable = true;

				var body = Self.GetComponent<RectBodyComponent>();
				var angle = body.Acceleration.LengthSquared() > 0.1f 
					?	body.Acceleration.ToAngle() 
					: (Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition) - Self.Center).ToAngle();

				direction = new Vector2((float) Math.Cos(angle) * RollForce, (float) Math.Sin(angle) * RollForce);
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Self.Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Self.Area.Add(part);
				}
			}

			public override void Destroy() {
				base.Destroy();
				
				Self.GetComponent<HealthComponent>().Unhittable = wasUnhittable;
				Self.GetComponent<RectBodyComponent>().Acceleration = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= RollTime) {
					Become<IdleState>();
					return;
				}
				
				var body = Self.GetComponent<RectBodyComponent>();
				body.Velocity = direction * (RollTime - T * 0.5f);
				body.Position += body.Velocity * dt * 0.75f;

				lastParticle -= dt;

				if (lastParticle <= 0) {
					lastParticle = 0.1f;
					
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Self.Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Self.Area.Add(part);
				}
			}

			public void ChangeDirection() {
				direction *= -1;
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
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Area.Add(part);
				}
				
				return true;
			} else if (e is RoomChangedEvent c) {
				c.New.Discover();

				// Camera following current room, felt weird
				/*if (Camera.Instance != null) {
					foreach (var target in Camera.Instance.Targets) {
						if (target.Entity == c.Old) {
							Camera.Instance.Targets.Remove(target);
							break;
						}
					}
					
					// I don't think the if clause worked right, connection rooms still were targeted
					if (c.New.Type != RoomType.Connection) {
						Camera.Instance.Targets.Add(new Camera.Target(c.New, 0.2f));
					}
				}*/
			}
			
			return base.HandleEvent(e);
		}

		public void RenderOutline() {
			var component = GetComponent<PlayerGraphicsComponent>();
			var color = component.Tint;
			
			component.Tint = new Color(0f, 0f, 0f, 0.65f);
			component.SimpleRender(false);
			component.Tint = color;
		}

		public override bool ShouldCollideWithDestroyableInAir() {
			return true;
		}

		protected override void HandleDeath() {
			Done = false;
			died = true;
		}

		private bool died;

		public override void AnimateDeath() {
			base.AnimateDeath();
			
			for (var i = 0; i < 6; i++) {
				Area.Add(new ParticleEntity(Particles.Dust()) {
					Position = Center + new Vector2(Random.Int(-4, 4), Random.Int(-4, 4)), 
					Depth = 30
				});
			}

			GetComponent<OrbitGiverComponent>().DestroyAll();
			GetComponent<FollowerComponent>().DestroyAll();
			
			var stone = new Tombstone();
			Area.Add(stone);
				
			stone.CenterX = CenterX;
			stone.Bottom = Bottom;
		}

		public void ModifyDrops(List<Item> drops) {
			var inventory = GetComponent<InventoryComponent>();

			foreach (var i in inventory.Items) {
				drops.Add(i);
			}

			inventory.Items.Clear();

			foreach (var c in Components.Values) {
				if (c is ItemComponent i && i.Item != null) {
					drops.Add(Items.Create(i.Item.Id));
				}
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Run.StartingNew || Run.StartedNew) {
				StartingWeapon = GetComponent<ActiveWeaponComponent>().Item?.Id;
				StartingLamp = GetComponent<LampComponent>().Item?.Id;
			}
		}
	}
}