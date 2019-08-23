using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, DropModifier {
		public static string StartingLamp;
		public static string StartingWeapon;

		public List<TextureRegion> PickedUp = new List<TextureRegion>();
		public float LastPickup;
		public Vector2 Scale;
		public Item PickedItem;

		public void AnimateItemPickup(Item item, Action action = null, bool add = true, bool ban = true) {
			Tween.To(1, 0, x => {
				Scale.X = x;
				Scale.Y = x;
			}, 0.2f);

			if (ban) {
				var banner = new UiDescriptionBanner();
				banner.Show(item);
				Engine.Instance.State.Ui.Add(banner);
			}
			
			PickedItem = item;
			
			Timer.Add(() => {
				Tween.To(0, 1, x => {
					Scale.X = x;
					Scale.Y = x;
				}, 0.2f, Ease.BackIn).OnEnd = () => {
					item.Area?.Remove(item);
					item.Done = false;
					PickedItem = null;

					if (add) {
						GetComponent<InventoryComponent>().Add(item);
					}

					action?.Invoke();
				};
			}, 1f);
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Height = 11;
			
			// Graphics
			// AddComponent(new LightComponent(this, 128f, new Color(0.3f, 0.4f, 0.3f, 0.2f)));
			
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
			AddComponent(new StatsComponent());
			AddComponent(new HeartsComponent());

			// Collisions
			AddComponent(new RectBodyComponent(4, 3, 8, 9));
			AddComponent(new InteractorComponent {
				CanInteractCallback = e => !died && PickedItem == null
			});

			// Other mechanics
			AddComponent(new OrbitGiverComponent());
			AddComponent(new FollowerComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;

			var hp = GetComponent<HealthComponent>();
			hp.MaxHealth = 1;
			hp.MaxHealthCap = 321;
			hp.HealthModifier = 5;

			if (Engine.Version.Dev) {
				Log.Info("Entering god mode for the player");
				hp.Unhittable = true;
			}
		}

		private int lastDepth = -3;

		public void FindSpawnPoint() {
			if (Run.StartedNew) {
				if (StartingWeapon == null) {
					StartingWeapon = Items.Generate(ItemPool.StartingWeapon, item => Item.Unlocked(item.Id));
				}

				if (StartingWeapon != null) {
					GetComponent<ActiveWeaponComponent>().Set(Items.CreateAndAdd(StartingWeapon, Area), false);
					StartingWeapon = null;
				}

				if (StartingLamp != null) {
					GetComponent<LampComponent>().Set(Items.CreateAndAdd(StartingLamp, Area), false);
					StartingLamp = null;
				}
			}
			
			if (lastDepth == Run.Depth) {
				return;
			}

			lastDepth = Run.Depth;
			
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
		
		public class DuckState : EntityState {
			
		}
		
		public class RunState : EntityState {
			private float lastParticle = 0.25f;
			
			
			public override void Update(float dt) {
				base.Update(dt);

				lastParticle -= dt;

				if (lastParticle <= 0) {
					lastParticle = 0.25f;
					
					var part = new ParticleEntity(Particles.Dust());
					
					part.Position = Self.Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Self.Area.Add(part);
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

		public override void Update(float dt) {
			base.Update(dt);

			if (PickedUp.Count > 0) {
				LastPickup += dt;

				if (LastPickup > 1f) {
					LastPickup -= 0.1f;
					PickedUp.RemoveAt(0);
				}
			}
		}

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Player) && base.ShouldCollide(entity);
		}

		public override bool InAir() {
			return base.InAir() || GetComponent<StateComponent>().StateInstance is RollState;
		}

		public override bool HasNoHealth(HealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == (e == null ? 0 : (e.Default ? 0 : -e.Amount));
		}
		
		public override bool HasNoHealth(PostHealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == (e == null ? 0 : (e.Default ? 0 : -e.Amount));
		}

		public override bool HandleEvent(Event e) {
			if (e is LostSupportEvent) {
				GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level.Chasm);
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Area.Add(part);
				}
			} else if (e is RoomChangedEvent c) {
				c.New.Discover();

				foreach (var i in c.New.Tagged[Tags.Item]) {
					if (i is ShopStand s) {
						s.Recalculate();
					}
				}

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
			} else if (e is PostHealthModifiedEvent h) {
				if (h.Amount < 0) {
					var cl = GetBloodColor();
					
					if (Random.Chance(30)) {
						for (var i = 0; i < Random.Int(1, 3); i++) {
							Area.Add(new SplashParticle {
								Position = Center - new Vector2(2.5f),
								Color = cl
							});
						}
					}

					Area.Add(new SplashFx {
						Position = Center,
						Color = ColorUtils.Mod(cl)
					});
				}
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

		protected override bool HandleDeath(DiedEvent d) {
			Done = false;
			died = true;

			return true;
		}

		public override bool IgnoresProjectiles() {
			return GetComponent<StateComponent>().StateInstance is RollState;
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

			var pool = new List<string>();

			foreach (var i in GetComponent<InventoryComponent>().Items) {
				pool.Add(i.Id);
			}

			var w = GetComponent<ActiveItemComponent>().Item;

			if (w != null) {
				pool.Add(w.Id);
			}

			w = GetComponent<WeaponComponent>().Item;

			if (w != null) {
				pool.Add(w.Id);
			}
			
			w = GetComponent<ActiveWeaponComponent>().Item;

			if (w != null) {
				pool.Add(w.Id);
			}

			if (pool.Count == 0) {
				pool.Add("bk:coin");
			}

			GlobalSave.Put("next_tomb", pool[Random.Int(pool.Count)]);
			GlobalSave.Put("tomb_depth", Run.Depth);
			
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