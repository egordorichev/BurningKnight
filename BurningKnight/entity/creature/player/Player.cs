using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.debug;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
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
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, DropModifier {
		public static string StartingWeapon;
		public static string StartingItem;

		public List<TextureRegion> PickedUp = new List<TextureRegion>();
		public float LastPickup;
		public Vector2 Scale;
		public Item PickedItem;

		private bool dead;

		public void AnimateItemPickup(Item item, Action action = null, bool add = true, bool ban = true) {
			if (ban) {
				var banner = new UiDescriptionBanner();
				banner.Show(item);
				Engine.Instance.State.Ui.Add(banner);
			}

			if (add || item.Type == ItemType.Active || item.Type == ItemType.Weapon) {
				Engine.Instance.State.Ui.Add(new ConsumableParticle(item.Region, this, item.Type != ItemType.Active, () => {
					item.Area?.Remove(item);
					item.Done = false;
					PickedItem = null;
					
					action?.Invoke();

					if (item.Type != ItemType.Active && item.Type != ItemType.Weapon) {
						GetComponent<InventoryComponent>().Add(item);
					}
				}));
				
				return;
			}
			
			Tween.To(1, 0, x => {
				Scale.X = x;
				Scale.Y = x;
			}, 0.2f);

			PickedItem = item;
			
			Timer.Add(() => {
				Tween.To(0, 1, x => {
					Scale.X = x;
					Scale.Y = x;
				}, 0.2f, Ease.BackIn).OnEnd = () => {
					item.Area?.Remove(item);
					item.Done = false;
					PickedItem = null;

					action?.Invoke();
				};
			}, 1f);
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Height = 11;
			
			// Graphics
			AddComponent(new LightComponent(this, 64, new Color(1f, 0.8f, 0.6f, 1f)));
			
			AddComponent(new PlayerGraphicsComponent {
				Offset = new Vector2(0, -5)
			});
			
			// Inventory
			AddComponent(new InventoryComponent());
			AddComponent(new ActiveItemComponent());
			AddComponent(new ActiveWeaponComponent());
			AddComponent(new WeaponComponent());
			AddComponent(new ConsumablesComponent());
			AddComponent(new HatComponent());
			
			// Stats
			AddComponent(new HeartsComponent());
			AddComponent(new StatsComponent());

			// Collisions
			AddComponent(new RectBodyComponent(4, Height - 1, 8, 1) {
				CanCollide = false
			});
			
			AddComponent(new SensorBodyComponent(2, 1, Width - 4, Height - 1, BodyType.Dynamic, true));
			GetComponent<SensorBodyComponent>().Body.SleepingAllowed = false;
			
			AddComponent(new InteractorComponent {
				CanInteractCallback = e => !died && PickedItem == null
			});

			// Other mechanics
			AddComponent(new OrbitGiverComponent());
			AddComponent(new FollowerComponent());
			AddComponent(new AimComponent(AimComponent.AimType.Cursor));
			AddComponent(new DialogComponent());
			
			AddComponent(new ZComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;

			InitStats();
			
			Subscribe<RoomClearedEvent>();
		}

		public void InitStats() {
			GetComponent<OrbitGiverComponent>().DestroyAll();
			GetComponent<FollowerComponent>().DestroyAll();

			var hp = GetComponent<HealthComponent>();
			hp.MaxHealth = 6;
			hp.InitMaxHealth = 6;
			hp.MaxHealthCap = 32;
			hp.InvincibilityTimerMax = 1f;

			if (CheatWindow.AutoGodMode) {
				Log.Info("Entering god mode for the player");
				hp.Unhittable = true;
			}
		}

		private int lastDepth = -3;

		public void FindSpawnPoint() {
			if (Run.StartedNew && Run.Depth > 0) {
				if (StartingWeapon == null) {
					StartingWeapon = Items.Generate(ItemPool.StartingWeapon, item => Item.Unlocked(item.Id));
				}

				if (StartingWeapon != null) {
					GetComponent<ActiveWeaponComponent>().Set(Items.CreateAndAdd(StartingWeapon, Area), false);
				}
				
				if (StartingItem != null) {
					GetComponent<ActiveItemComponent>().Set(Items.CreateAndAdd(StartingItem, Area), false);
				}
			}
			
			if (lastDepth == Run.Depth) {
				return;
			}

			lastDepth = Run.Depth;
			
			foreach (var c in Area.Tagged[Tags.Checkpoint]) {
				Center = c.Center;
				Log.Debug("Teleported to spawn point");
				return;
			}

			foreach (var c in Area.Tagged[Tags.Entrance]) {
				Center = c.Center + new Vector2(0, 4);
				Log.Debug("Teleported to entrance");
				return;
			}
			
			foreach (var r in Area.Tagged[Tags.Room]) {
				var rm = (Room) r;

				if (rm.Type == RoomType.Entrance) {
					Center = r.Center;
					rm.Discover();

					return;
				}
			}
			
			foreach (var r in Area.Tagged[Tags.Room]) {
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
			public override void Init() {
				base.Init();

				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("quck");

				Self.HandleEvent(new QuackEvent {
					Player = (Player) Self
				});
			}

			public override void Update(float dt) {
				base.Update(dt);

				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.GetComponent<RectBodyComponent>().Acceleration = Vector2.Zero;
			}
		}
		
		public class RunState : EntityState {
			private float lastParticle = 0.25f;
			private uint lastFrame;
			
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

				var anim = Self.GetComponent<PlayerGraphicsComponent>().Animation;

				if (anim.Frame != lastFrame) {
					lastFrame = anim.Frame;

					if (lastFrame == 2 || lastFrame == 6) {
						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("step");
					}
				}
			}
		}
		
		public class RollState : EntityState {
			private const float RollTime = 0.25f;
			private const float RollForce = 1600f;
			
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
				body.Velocity += direction * (RollTime - T * 0.5f);
				body.Position += body.Velocity * dt * 0.1f;

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
			return !(entity is Player || ((entity is ItemStand || entity is Bomb) && InAir())) && base.ShouldCollide(entity);
		}

		public bool HasFlight;

		public override bool InAir() {
			return HasFlight || base.InAir() || GetComponent<StateComponent>().StateInstance is RollState;
		}
		
		/*
		public override bool HasNoHealth(HealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == (e == null ? 0 : (e.Default ? 0 : -e.Amount));
		}
		
		public override bool HasNoHealth(PostHealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == 0;
		}*/

		public override bool HandleEvent(Event e) {
			if (e is LostSupportEvent) {
				if (GetComponent<HealthComponent>().Unhittable) {
					return true;
				}
				
				GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level);
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Area.Add(part);
				}
			} else if (e is RoomChangedEvent c) {
				if (c.New == null) {
					return base.HandleEvent(e);
				}
				
				c.New.Discover();

				switch (c.New.Type) {
					case RoomType.Secret:
					case RoomType.Special:
					case RoomType.Shop:
					case RoomType.Treasure: {
						ExplosionMaker.CheckForCracks(Run.Level, c.New, this);
					
						foreach (var door in c.New.Doors) {
							if (door.TryGetComponent<LockComponent>(out var component) && component.Lock is GoldLock) {
								component.Lock.SetLocked(false, this);
							}
						}
						
						break;
					}
				}

				if (c.Old != null) {
					Camera.Instance.Unfollow(c.Old);
				}

				if (c.New != null && c.New.Tagged[Tags.MustBeKilled].Count > 0) {
					Camera.Instance.Follow(c.New, 0.3f);
				}
			} else if (e is HealthModifiedEvent hm) {
				if (hm.Amount < 0) {
					if (hm.From is Mob m && m.HasPrefix) {
						hm.Amount = Math.Min(hm.Amount, -2);
					}

					hm.Amount = Math.Max(-1, hm.Amount);
				}			
			} else if (e is PostHealthModifiedEvent h) {
				if (h.Amount < 0) {
					HandleEvent(new PlayerHurtEvent {
						Player = this
					});

					if (Settings.Blood) {
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
			} else if (e is RoomClearedEvent rce) {
				Camera.Instance.Unfollow(rce.Room);
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
			return !HasFlight;
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

		public override void AnimateDeath(DiedEvent d) {
			dead = true;
			
			base.AnimateDeath(d);
			
			for (var i = 0; i < 6; i++) {
				Area.Add(new ParticleEntity(Particles.Dust()) {
					Position = Center + new Vector2(Random.Int(-4, 4), Random.Int(-4, 4)), 
					Depth = 30
				});
			}

			GetComponent<OrbitGiverComponent>().DestroyAll();
			GetComponent<FollowerComponent>().DestroyAll();
			
			var stone = new Tombstone();
			stone.DisableDialog = true;

			var pool = new List<string>();

			foreach (var i in GetComponent<InventoryComponent>().Items) {
				if (i.Type != ItemType.Hat) {
					pool.Add(i.Id);
				}
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
			
			Camera.Instance.Targets.Clear();
			Camera.Instance.Follow(stone, 0.5f);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt32(lastDepth);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			lastDepth = stream.ReadInt32();
		}

		public void ModifyDrops(List<Item> drops) {
			if (!dead) {
				return;
			}
			
			var inventory = GetComponent<InventoryComponent>();

			foreach (var i in inventory.Items) {
				drops.Add(i);
			}

			inventory.Items.Clear();

			foreach (var c in Components.Values) {
				if (c is ItemComponent i && i.Item != null && i.Item.Type != ItemType.Hat) {
					drops.Add(Items.Create(i.Item.Id));
				}
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Run.LastDepth == -1 || Run.LastDepth == 0) {
				StartingWeapon = GetComponent<ActiveWeaponComponent>().Item?.Id;
				StartingItem = GetComponent<ActiveItemComponent>().Item?.Id;
			}
		}
	}
}