using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.debug;
using BurningKnight.entity.bomb;
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
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, DropModifier {
		public static string StartingWeapon;
		public static string StartingItem;

		private bool dead;

		public void AnimateItemPickup(Item item, Action action = null, bool add = true, bool ban = true) {
			if (ban) {
				var banner = new UiDescriptionBanner();
				banner.Show(item);
				Engine.Instance.State.Ui.Add(banner);
			}
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("item_pickup");

			if (add || item.Type == ItemType.Active || item.Type == ItemType.ConsumableArtifact || item.Type == ItemType.Weapon || item.Type == ItemType.Hat) {
				GetComponent<InventoryComponent>().Busy = true;
				
				Engine.Instance.State.Ui.Add(new ConsumableParticle(item.Region, this, item.Type != ItemType.Active, () => {
					item.Area?.Remove(item);
					item.Done = false;
					
					action?.Invoke();
					GetComponent<InventoryComponent>().Busy = false;

					if (item.Type != ItemType.ConsumableArtifact && item.Type != ItemType.Active && item.Type != ItemType.Weapon && item.Type != ItemType.Hat) {
						GetComponent<InventoryComponent>().Add(item);
					}
				}));
				
				return;
			}
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
				CanInteractCallback = e => !died
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

			InitStats(true);
			
			Subscribe<RoomClearedEvent>();
		}

		public void InitStats(bool fromInit = false) {
			HasFlight = false;

			GetComponent<AimComponent>().ShowLaserLine = false;
			GetComponent<OrbitGiverComponent>().DestroyAll();
			GetComponent<FollowerComponent>().DestroyAll();

			var hp = GetComponent<HealthComponent>();
			hp.InitMaxHealth = 6 - (fromInit ? 0 : GetComponent<StatsComponent>().HeartsPayed * 2);
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
					var i = Items.CreateAndAdd(StartingWeapon, Area);
					i.Scourged = false;
					GetComponent<ActiveWeaponComponent>().Set(i, false);
				}
				
				if (StartingItem != null) {
					var i = Items.CreateAndAdd(StartingItem, Area);
					i.Scourged = false;
					GetComponent<ActiveItemComponent>().Set(i, false);
				}
			}
			
			if (lastDepth == Run.Depth) {
				return;
			}

			lastDepth = Run.Depth;
			HandleEvent(new NewLevelStartedEvent());
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
					part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
					Self.Area.Add(part);
				}

				var anim = Self.GetComponent<PlayerGraphicsComponent>().Animation;

				if (anim.Frame != lastFrame) {
					lastFrame = anim.Frame;

					if (Run.Level != null && (lastFrame == 2 || lastFrame == 6)) {
						var x = (int) (Self.CenterX / 16);
						var y = (int) (Self.Bottom / 16);

						if (!Run.Level.IsInside(x, y)) {
							return;
						}

						var i = Run.Level.ToIndex(x, y);
						var tile = Run.Level.Get(i);
						var liquid = Run.Level.Liquid[i];

						Audio.PlaySfx(Run.Level.Biome.GetStepSound(liquid == 0 ? tile : (Tile) liquid), 0.25f);
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
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("player_roll", 0.5f);
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
					part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
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
					part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
					Self.Area.Add(part);
				}
			}

			public void ChangeDirection() {
				direction *= -1;
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Player || ((entity is ItemStand || entity is Bomb) && InAir())) && base.ShouldCollide(entity);
		}

		public bool HasFlight;

		public override bool InAir() {
			return HasFlight || base.InAir() || GetComponent<StateComponent>().StateInstance is RollState;
		}
		
		public override bool HasNoHealth(HealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == 0;
		}
		
		public override bool HasNoHealth(PostHealthModifiedEvent e = null) {
			return base.HasNoHealth(e) && GetComponent<HeartsComponent>().Total == 0;
		}

		public override bool HandleEvent(Event e) {
			if (e is LostSupportEvent) {
				if (GetComponent<HealthComponent>().Unhittable) {
					return true;
				}
				
				GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level);
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
					Area.Add(part);
				}
			} else if (e is RoomChangedEvent c) {
				if (c.New == null || Run.Level == null || Camera.Instance == null) {
					return base.HandleEvent(e);
				}
				
				if (c.New.Tagged[Tags.MustBeKilled].Count > 0) {
					Audio.PlaySfx("level_door_shut");
				}

				if (c.Old != null && Scourge.IsEnabled(Scourge.OfLost)) {
					c.Old.Hide();
				}
				
				c.New.Discover();
				var level = Run.Level;

				if (InGameState.Ready) {
					switch (c.New.Type) {
						case RoomType.Secret:
						case RoomType.Special:
						case RoomType.Shop:
						case RoomType.SubShop:
						case RoomType.Treasure: {
							foreach (var door in c.New.Doors) {
								if (door.TryGetComponent<LockComponent>(out var component) && component.Lock is GoldLock) {
									if (!(c.New.Type == RoomType.Shop && ((door.Rooms[0] != null && door.Rooms[0].Type == RoomType.SubShop) ||
									                                    (door.Rooms[1] != null && door.Rooms[1].Type == RoomType.SubShop)))) {
									
										component.Lock.SetLocked(false, this);
									} 
									
								}
							}

							break;
						}

						case RoomType.OldMan:
						case RoomType.Granny: {
							if (c.New.Type == RoomType.OldMan) {
								GetComponent<StatsComponent>().SawDeal = true;
							}

							c.New.OpenHiddenDoors();
							
							foreach (var r in Area.Tagged[Tags.Room]) {
								var room = (Room) r;

								if (room.Type == (c.New.Type == RoomType.OldMan ? RoomType.Granny : RoomType.OldMan)) {
									room.CloseHiddenDoors();
									break;
								}
							}
							
							break;
						}
					}
					
					if (c.New.Type == RoomType.Secret) {
						ExplosionMaker.CheckForCracks(level, c.New, this);
					}
				}
				
				if (c.Old != null && c.Old.Type == RoomType.OldMan) {
					c.Old.CloseHiddenDoors();
				}

				// Darken the lighting in evil rooms
				if (c.New.Type == RoomType.OldMan || c.New.Type == RoomType.Spiked) {
					Tween.To(0.7f, Lights.RadiusMod, x => Lights.RadiusMod = x, 0.3f);
				} else if (c.Old != null && (c.Old.Type == RoomType.OldMan || c.Old.Type == RoomType.Spiked)) {
					Tween.To(1f, Lights.RadiusMod, x => Lights.RadiusMod = x, 0.3f);
				}

				/*if (c.Old != null) {
					Camera.Instance.Unfollow(c.Old);
				}

				if (c.New != null && c.New.Tagged[Tags.MustBeKilled].Count > 0) {
					Camera.Instance.Follow(c.New, 0.3f);
				}*/
			} else if (e is HealthModifiedEvent hm) {
				if (hm.Amount < 0) {
					if (hm.From is Mob m && m.HasPrefix) {
						hm.Amount = Math.Min(hm.Amount, -2);
					} else if (hm.Type != DamageType.Custom) {
						hm.Amount = Math.Max(-1, hm.Amount);
					}
				}			
			} else if (e is PostHealthModifiedEvent h) {
				if (h.Amount < 0) {
					HandleEvent(new PlayerHurtEvent {
						Player = this
					});

					Audio.PlaySfx("player_hurt", 1f);

					if (Settings.Blood) {
						var cl = GetBloodColor();

						if (Rnd.Chance(30)) {
							for (var i = 0; i < Rnd.Int(1, 3); i++) {
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
				Audio.PlaySfx("level_room_cleared", 0.25f + Audio.Db3);
			} else if (e is NewLevelStartedEvent) {
				foreach (var cc in Area.Tagged[Tags.Checkpoint]) {
					Center = cc.Center;
					Log.Debug("Teleported to spawn point");
					return base.HandleEvent(e);
				}

				foreach (var cc in Area.Tagged[Tags.Entrance]) {
					Center = cc.Center + new Vector2(0, 4);
					Log.Debug("Teleported to entrance");
					return base.HandleEvent(e);
				}
			
				foreach (var r in Area.Tagged[Tags.Room]) {
					var rm = (Room) r;

					if (rm.Type == RoomType.Entrance) {
						Center = r.Center;
						rm.Discover();

						return base.HandleEvent(e);
					}
				}
			
				foreach (var r in Area.Tagged[Tags.Room]) {
					var rm = (Room) r;
				
					if (rm.Type == RoomType.Exit) {
						Center = new Vector2(rm.CenterX, rm.Bottom - 1.4f * 16);
						rm.Discover();

						return base.HandleEvent(e);
					}
				}
			
				Log.Error("Did not find a spawn point!");
			} else if (e is ProjectileCreatedEvent pce) {
				if (Flying || HasFlight) {
					pce.Projectile.Spectral = true;
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
					Position = Center + new Vector2(Rnd.Int(-4, 4), Rnd.Int(-4, 4)), 
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

			GlobalSave.Put("next_tomb", pool[Rnd.Int(pool.Count)]);
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

		protected override string GetHurtSfx() {
			return null;
		}

		protected override string GetDeadSfx() {
			return null;
		}
	}
}