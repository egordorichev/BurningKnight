using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.assets.particle.renderer;
using BurningKnight.debug;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
using BurningKnight.entity.twitch;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.gamerenderer;
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
		private static Color tint = new Color(50, 234, 60, 200);

		public const int MaxPlayers = 5;

		public static Color[] IndexTints = {
			Palette.Default[59],
			Palette.Default[42],
			Palette.Default[35],
			Palette.Default[55],
			Palette.Default[31]
		};

		public static Vector4[] VectorTints;

		static Player() {
			VectorTints = new Vector4[MaxPlayers];
			
			for (var i = 0; i < MaxPlayers; i++) {
				var color = IndexTints[i];
				VectorTints[i] = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
			}
		}
		
		public Color Tint => IndexTints[GetComponent<InputComponent>().Index];
		
		public static int Quacks;
		public static bool ToBoss;
		public static bool InBuilding;
		public static Color LightColor = new Color(1f, 0.8f, 0.6f, 1f);
		
		public static string[] StartingWeapons = new string[MaxPlayers];
		public static string[] StartingItems = new string[MaxPlayers];
		public static string[] StartingLamps = new string[MaxPlayers];
		public static List<string> DailyItems;
		public string ProjectileTexture = "rect";

		public bool ItemDamage;
		public bool Sliding;

		public bool Dead;

		public void AnimateItemPickup(Item item, Action action = null, bool add = true, bool ban = true) {
			if (ban) {
				var banner = new UiDescriptionBanner();
				banner.Show(item);
				Engine.Instance.State.Ui.Add(banner);
			}

			if (item.Type == ItemType.Weapon && Run.Depth == 0) {
				Audio.PlaySfx(item.Data.WeaponType.GetPickupSfx());
			} else {
				GetComponent<AudioEmitterComponent>().EmitRandomized("item_pickup");
			}

			if (add || item.Type == ItemType.Lamp || item.Type == ItemType.Active || item.Type == ItemType.ConsumableArtifact || item.Type == ItemType.Weapon || item.Type == ItemType.Hat) {
				GetComponent<InventoryComponent>().Busy = true;
				
				Engine.Instance.State.Ui.Add(new ConsumableParticle(item.Region, this, item.Type != ItemType.Active, () => {
					item.Area?.Remove(item);
					item.Done = false;
					
					action?.Invoke();
					GetComponent<InventoryComponent>().Busy = false;

					if (item.Type != ItemType.ConsumableArtifact && item.Type != ItemType.Active && item.Type != ItemType.Weapon && item.Type != ItemType.Hat && item.Type != ItemType.Lamp) {
						GetComponent<InventoryComponent>().Add(item);
					}
				}));
				
				return;
			}
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new InputComponent());
			AddComponent(new CursorComponent());
			
			InBuilding = false;
			GetComponent<HealthComponent>().SaveMaxHp = true;
			
			Height = 11;
			
			// Graphics
			if (Run.Depth != 0) {
				AddComponent(new LightComponent(this, 64, LightColor));
			}

			AddComponent(new PlayerGraphicsComponent {
				Offset = new Vector2(0, -5)
			});
			
			// Inventory
			AddComponent(new LampComponent());
			AddComponent(new InventoryComponent());
			AddComponent(new ActiveItemComponent());
			AddComponent(new ActiveWeaponComponent());
			AddComponent(new WeaponComponent());
			AddComponent(new ConsumablesComponent());
			AddComponent(new HatComponent());
			
			// Stats
			AddComponent(new ManaComponent());
			AddComponent(new HeartsComponent());
			AddComponent(new StatsComponent());

			// Collisions
			AddComponent(new RectBodyComponent(4, Height - 1, 8, 1) {
				CanCollide = false
			});
			
			AddComponent(new SensorBodyComponent(3, 2, Width - 6, Height - 3, BodyType.Dynamic, true));
			GetComponent<SensorBodyComponent>().Body.SleepingAllowed = false;
			
			AddComponent(new InteractorComponent {
				CanInteractCallback = e => !died && !GetComponent<InventoryComponent>().Busy
			});

			// Other mechanics
			AddComponent(new OrbitGiverComponent());
			AddComponent(new FollowerComponent());
			AddComponent(new AimComponent(AimComponent.AimType.Cursor));
			AddComponent(new DialogComponent());
			
			AddComponent(new ZComponent());
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerTarget);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;

			InitStats(true);
			
			Subscribe<NewFloorEvent>();
		}

		public void InitStats(bool fromInit = false) {
			HasFlight = false;
			SuperHot = false;

			Scourge.Clear();

			GetComponent<AimComponent>().ShowLaserLine = false;
			GetComponent<OrbitGiverComponent>().DestroyAll();
			GetComponent<FollowerComponent>().DestroyAll();

			var hp = GetComponent<HealthComponent>();

			if (fromInit) {
				hp.InitMaxHealth = 6;
			}

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
				TwitchBridge.OnNewRun?.Invoke();

				var index = GetComponent<InputComponent>().Index;
				
				if (StartingLamps[index] != null) {
					var i = Items.CreateAndAdd(StartingLamps[index], Area);
					i.Scourged = false;
					GetComponent<LampComponent>().Set(i, false);
					Log.Debug($"Starting lamp: {StartingLamps[index]}");
				}
				
				if (StartingWeapons[index] == null || !ItemPool.StartingWeapon.Contains(Items.Datas[StartingWeapons[index]].Pools)) {
					StartingWeapons[index] = Items.Generate(ItemPool.StartingWeapon, item => Item.Unlocked(item.Id));
				}

				if (StartingWeapons[index] != null) {
					var i = Items.CreateAndAdd(StartingWeapons[index], Area);
					i.Scourged = false;

					var l = GetComponent<LampComponent>().Item;

					if (l != null && l.Id == "bk:sharp_lamp" && i.Data.WeaponType != WeaponType.Melee) {
						StartingWeapons[index] = Items.Generate(ItemPool.StartingWeapon, item => Item.Unlocked(item.Id));
						i.Done = true;
						i = Items.CreateAndAdd(StartingWeapons[index], Area);
					}
					
					GetComponent<ActiveWeaponComponent>().Set(i, false);
					Log.Debug($"Starting weapon: {StartingWeapons[index]}");
				}
				
				if (StartingItems[index] != null) {
					var i = Items.CreateAndAdd(StartingItems[index], Area);
					i.Scourged = false;
					GetComponent<ActiveItemComponent>().Set(i, false);
					
					Log.Debug($"Starting item: {StartingItems[index]}");
				}

				if (DailyItems != null) {
					if (Run.Type == RunType.Daily) {
						var inventory = GetComponent<InventoryComponent>();
						
						foreach (var id in DailyItems) {
							Log.Info($"Giving {id}");
							inventory.Pickup(Items.CreateAndAdd(id, Area), false);
						}
					}
					
					DailyItems = null;
				}
			}

			findASpawn = true;

			if (lastDepth == Run.Depth) {
				Log.Info("Old depth is the same as the current one");
				return;
			}

			lastDepth = Run.Depth;
			
			if (Run.Depth > 1 && !GetComponent<StatsComponent>().TookDamageOnLevel) {
				Achievements.Unlock("bk:dodge_overlord");
			}
			
			HandleEvent(new NewLevelStartedEvent());
		}

		private bool set;
		private float t;
		private bool findASpawn;
		public bool Teleported;

		private bool FindSpawn() {
			if (/*BK.Version.Dev || */ToBoss) {
				ToBoss = false;
				
				foreach (var r in Area.Tagged[Tags.Room]) {
					var rm = (Room) r;

					if (rm.Type == RoomType.Boss) {
						Center = r.Center + new Vector2(0, 32) + Rnd.Vector(-0.5f, 0.5f);
						rm.Discover();
						Log.Debug("Teleported to boss room");
						return true;
					}
				}
			}
			
			foreach (var cc in Area.Tagged[Tags.Checkpoint]) {
				Center = cc.Center + Rnd.Vector(-0.5f, 0.5f);
				Log.Debug("Teleported to spawn point");
				return true;
			}

			foreach (var cc in Area.Tagged[Tags.Entrance]) {
				Center = cc.Center + new Vector2(0, 4) + Rnd.Vector(-0.5f, 0.5f);
				Log.Debug("Teleported to entrance");
				return true;
			}

			foreach (var r in Area.Tagged[Tags.Room]) {
				var rm = (Room) r;

				if (rm.Type == RoomType.Entrance) {
					Center = r.Center + Rnd.Vector(-0.5f, 0.5f);
					rm.Discover();
					Log.Debug("Teleported to entrance room");
					return true;
				}
			}

			foreach (var r in Area.Tagged[Tags.Room]) {
				var rm = (Room) r;

				if (rm.Type == RoomType.Exit) {
					Log.Debug("Teleported to exit room");
					Center = new Vector2(rm.CenterX, rm.Bottom - 1.4f * 16) + Rnd.Vector(-0.5f, 0.5f);
					rm.Discover();

					return true;
				}
			}
			
			
			foreach (var r in Area.Tagged[Tags.Room]) {
				var rm = (Room) r;

				Log.Debug("Teleported to random room");
				Center = new Vector2(rm.CenterX, rm.Bottom - 1.4f * 16) + Rnd.Vector(-0.5f, 0.5f);
				rm.Discover();

				return true;
			}

			Log.Error("Failed to teleport!");
			AnimationUtil.Poof(Center, 20);
			return false;
		}
		
		public override void Update(float dt) {
			if (findASpawn) {
				if (FindSpawn()) {
					Teleported = true;
					findASpawn = false;
					
					if (!CheatWindow.AutoGodMode) {
						GetComponent<HealthComponent>().Unhittable = false;
					}
				} else {
					Log.Error("Did not find a spawn point!");
				}
			}
			
			base.Update(dt);
			t += dt;

			if (!set && t >= 0.3f) {
				set = true;
				GetComponent<RoomComponent>().Room?.Discover();

				if (Run.Depth == 0) {
					CageLock.CheckProgress();
					HatStand.CheckHats();
					Items.CheckForCollector();
					Builder.CheckShortcutUnlocks();

					if (Assets.FailedToLoadAudio) {
						Assets.FailedToLoadAudio = false;
						((InGameState) Engine.Instance.State).TopUi.Add(new UiError("Audio Failed",	"Failed to init audio device"));
					}
				}
			}
		}

		#region Player States
		public class IdleState : EntityState {
			public override void Update(float dt) {
				base.Update(dt);

				if (!CheatWindow.NoSleep && T >= 6f && Self.HasComponent<PlayerInputComponent>()) {
					Become<SittingState>();
				}
			}
		}

		public class SittingState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<PlayerGraphicsComponent>().Animate();
			}
			
			public override void Destroy() {
				base.Destroy();
				
				var pr = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
				pr.EnableClip = false;
				Self.GetComponent<PlayerGraphicsComponent>().Animate();
			}
			
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 24f && Self.HasComponent<PlayerInputComponent>()) {
					Become<SleepingState>();
				}
			}
		}

		public class SleepingState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<PlayerGraphicsComponent>().Animate();
			}
			
			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<PlayerGraphicsComponent>().Animate();
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 3f) {
					T = 0;

					for (var i = 0; i < 3; i++) {
						Timer.Add(() => {
								var part = new ParticleEntity(new Particle(Controllers.Float,
									new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"sleep"))));

								part.Position = Self.Center;

								if (Self.TryGetComponent<ZComponent>(out var z)) {
									part.Position -= new Vector2(0, z.Z);
								}

								Self.Area.Add(part);

								part.Particle.Velocity = new Vector2(Rnd.Float(8, 16) * (Rnd.Chance() ? -1 : 1), -Rnd.Float(30, 56));
								part.Particle.Angle = 0;
								part.Particle.Alpha = 0.9f;
								part.Depth = Layers.InGameUi;
							}, i * 0.5f);
					}
				}
			}
		}

		public class DuckState : EntityState {
			public override void Init() {
				base.Init();
				Quacks++;
				if (Quacks >= 150) {
					Achievements.Unlock("bk:quackers");
				}

				var hat = Self.GetComponent<HatComponent>().Item;

				if (hat != null && (hat.Id == "bk:villager_head" || hat.Id == "bk:stone_hat")) {
					Audio.PlaySfx($"villager{Rnd.Int(1, 5)}", 1f, Rnd.Float(-0.5f, 0.5f));
				} else {
					Audio.PlaySfx("quck", 1f, Rnd.Float(-0.5f, 0.5f));
				}
				
				Self.HandleEvent(new QuackEvent {
					Player = (Player) Self
				});
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<RectBodyComponent>().Velocity.Length() > 10f) {
					if (T >= 0.2f) {
						AnimationUtil.Poof(Self.Center);
						T = 0;
					}
				} else {
					T = 0;
				}
			}
		}
		
		public class RunState : SmartState<Player> {
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

				if (!Self.HasFlight) {
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
							var room = Self.GetComponent<RoomComponent>().Room;

							Audio.PlaySfx(Run.Level.Biome.GetStepSound(liquid == 0 ? tile : (Tile) liquid),
								room != null && room.Tagged[Tags.MustBeKilled].Count > 0 ? 0.18f : 0.25f);
						}
					}
				}
			}
		}


		public class PostRollState : EntityState {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 0.2f) {
					Self.GetComponent<RectBodyComponent>().Acceleration = Vector2.Zero;
					Become<IdleState>();
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

				var z = Self.GetComponent<ZComponent>();
				var start = z.Z;

				Tween.To(start + 8, start, x => z.Z = x, 0.15f, Ease.QuadIn).OnEnd = () => {
					Tween.To(start, z.Z, x => z.Z = x, 0.15f, Ease.QuadIn);
				};
				
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
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= RollTime) {
					Become<PostRollState>();
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
			if (HasFlight && entity is HalfWall) {
				return false;
			}
			
			return !(entity is Player || entity is HalfProjectileLevel || entity is ProjectileLevelBody || ((entity is ItemStand || entity is Bomb) && InAir())) && base.ShouldCollide(entity);
		}

		public bool HasFlight;
		public bool SuperHot;

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

				if (!GetComponent<BuffsComponent>().PitImmunity) {
					GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level);
				}
				

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

					foreach (var p in Area.Tagged[Tags.Player]) {
						if (p.GetComponent<RoomComponent>().Room != c.New) {
							AnimationUtil.Poof(p.Center);
							p.Center = Center;
							AnimationUtil.Poof(p.Center);
						}
					}
				}

				((InGameState) Engine.Instance.State).ResetFollowing();

				var pr = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;

				if (c.Old != null) {
					if (Scourge.IsEnabled(Scourge.OfLost)) {
						c.Old.Hide();
					}
					
					if (c.Old.Type == RoomType.DarkMarket || c.Old.Type == RoomType.Hidden) {
						pr.EnableClip = false;
						c.Old.Hide(true);
						InBuilding = false;
						((InGameState) Engine.Instance.State).UpdateRainVolume();
					}
				}

				if (c.New.Type == RoomType.DarkMarket) {
					Achievements.Unlock("bk:dark_market");
				}
				
				if (c.New.Type == RoomType.DarkMarket || c.New.Type == RoomType.Hidden) {
					pr.EnableClip = true;
					pr.ClipPosition = new Vector2(c.New.X + 16, c.New.Y + 16);
					pr.ClipSize = new Vector2(c.New.Width - 16, c.New.Height - 32);
					InBuilding = true;
					((InGameState) Engine.Instance.State).UpdateRainVolume();
				} else {
					pr.EnableClip = false;
				}

				if (c.New.Type == RoomType.Shop) {
					Audio.PlaySfx("level_door_bell");
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
				
				if (c.Old != null) {
					if (c.Old.Type == RoomType.OldMan) {
						var found = false;

						foreach (var p in c.Old.Tagged[Tags.Player]) {
							if (p != this && p is Player) {
								found = true;

								break;
							}
						}

						if (!found) {
							c.Old.CloseHiddenDoors();
						}
					} else if (c.Old.Type == RoomType.Treasure && Run.Type != RunType.BossRush && !Rnd.Chance(5)) {
						var found = false;

						foreach (var p in c.Old.Tagged[Tags.Player]) {
							if (p != this && p is Player) {
								found = true;

								break;
							}
						}

						if (!found) {
							foreach (var door in c.Old.Doors) {
								var x = (int) Math.Floor(door.CenterX / 16);
								var y = (int) Math.Floor(door.Bottom / 16);
								var t = level.Get(x, y);

								if (level.Get(x, y).Matches(TileFlags.Passable)) {
									var index = level.ToIndex(x, y);

									level.Set(index, level.Biome is IceBiome ? Tile.WallB : Tile.WallA);
									level.UpdateTile(x, y);
									level.ReCreateBodyChunk(x, y);
									level.LoadPassable();

									Camera.Instance.Shake(10);
								}
							}

							c.Old.ApplyToEachTile((x, y) => {
								if (Run.Level.Get(x, y).IsWall()) {
									return;
								}

								Timer.Add(() => {
									var part = new TileParticle();

									part.Top = Run.Level.Tileset.WallTopADecor;
									part.TopTarget = Run.Level.Tileset.WallTopADecor;
									part.Side = Run.Level.Tileset.FloorSidesD[0];
									part.Sides = Run.Level.Tileset.WallSidesA[2];
									part.Tile = Tile.WallA;

									part.X = x * 16;
									part.Y = y * 16;
									part.Target.X = x * 16;
									part.Target.Y = y * 16;
									part.TargetZ = -8f;

									Area.Add(part);
								}, Rnd.Float(0.5f));
							});

							foreach (var d in c.Old.Doors) {
								d.Done = true;
							}

							c.Old.Done = true;
						}
					}
				}

				// Darken the lighting in evil rooms
				if (c.New.Type == RoomType.OldMan || c.New.Type == RoomType.Spiked) {
					Tween.To(0.7f, Lights.RadiusMod, x => Lights.RadiusMod = x, 0.3f);
				} else if (c.Old != null && (c.Old.Type == RoomType.OldMan || c.Old.Type == RoomType.Spiked)) {
					Tween.To(1f, Lights.RadiusMod, x => Lights.RadiusMod = x, 0.3f);
				}
			} else if (e is HealthModifiedEvent hm) {
				if (hm.Amount < 0) {
					if ((hm.From is Mob m && m.HasPrefix) || (hm.From is creature.bk.BurningKnight k && k.InFight) || hm.From is BkOrbital) {
						hm.Amount = Math.Min(hm.Amount, -2);
					} else if (hm.Type != DamageType.Custom && hm.Type != DamageType.Explosive) {
						hm.Amount = Math.Max(-1, hm.Amount);
					}
				}			
			} else if (e is PostHealthModifiedEvent h) {
				if (h.Amount < 0 && !h.PressedForBomb) {
					HandleEvent(new PlayerHurtEvent {
						Player = this
					});

					var hp = GetComponent<HealthComponent>().Health + GetComponent<HeartsComponent>().Total;

					if (hp > 0) {
						if (h.HealthType == HealthType.Shield) {
							Audio.PlaySfx("player_shield_hurt", 1f);
						} else {
							Audio.PlaySfx(hp < 2 ? "player_low_hp_hurt" : "player_hurt", 1f);
						}
					}

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

				if (Run.Depth > 0 && !alerted && CheckClear(Area)) {
					alerted = true;
					AnimationUtil.Confetti(Center);
					Audio.PlaySfx("level_cleared");
				}
			} else if (e is NewLevelStartedEvent) {
				GetComponent<HealthComponent>().Unhittable = true;
			} else if (e is ProjectileCreatedEvent pce) {
				if (Flying || HasFlight) {
					pce.Projectile.AddFlags(ProjectileFlags.FlyOverStones);
				}
			} else if (e is FlagCollisionStartEvent fcse) {
				if (fcse.Flag == Flag.Burning) {
					GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level);
				}
			} else if (e is RevivedEvent re) {
				AnimationUtil.TeleportAway(this, () => {
					FindSpawn();
					Camera.Instance.Jump();
					AnimationUtil.TeleportIn(this);
				});
			} else if (e is CollisionStartedEvent cse) {
				if (ItemDamage && cse.Entity is Item) {
					GetComponent<HealthComponent>().ModifyHealth(-1, cse.Entity, DamageType.Custom);
				}
			}
			
			return base.HandleEvent(e);
		}

		private bool alerted;
		
		public static bool CheckClear(Area area) {
			foreach (var r in area.Tagged[Tags.Room]) {
				var room = (Room) r;

				if ((room.Type == RoomType.Regular) && !room.Cleared) {
					return false;
				}
			}
			
			return true;
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
			var ing = (InGameState) Engine.Instance.State;

			ing.Killer.Animation = null;
			ing.Killer.Slice = null;
			ing.Killer.UseSlice = true;

			var b = GetComponent<RectBodyComponent>();
			
			b.Knockback = Vector2.Zero;
			b.Velocity = Vector2.Zero;
			
			if (d.From != null && d.From != this) {
				var anim = d.From.GetAnyComponent<AnimationComponent>();

				if (anim != null) {
					ing.Killer.Animation = Animations.Get(anim.Id)?.CreateAnimation();
					
					if (ing.Killer.Animation != null) {
						ing.Killer.Animation.Tag = "idle";
						ing.Killer.UseSlice = false;

						var c = ing.Killer.Animation.GetCurrentTexture();
						
						ing.Killer.Width = c.Width;
						ing.Killer.Height = c.Height;
					}
				} else {
					var slice = d.From.GetAnyComponent<SliceComponent>();

					if (slice != null) {
						ing.Killer.Slice = slice.Sprite;
						ing.Killer.Width = ing.Killer.Slice.Width;
						ing.Killer.Height = ing.Killer.Slice.Height;
					}
				}
			}

			if (d.From == this) {
				ing.Killer.Slice = CommonAse.Ui.GetSlice("self");
			}

			if (ing.Killer.Slice == null && ing.Killer.Animation == null) {
				ing.Killer.Slice = CommonAse.Items.GetSlice("unknown");
				ing.Killer.Width = ing.Killer.Slice.Width;
				ing.Killer.Height = ing.Killer.Slice.Height;
			}

			ing.Killer.Width *= 2;
			ing.Killer.Height *= 2;
			ing.Killer.RelativeCenterX = Display.UiWidth * 0.75f;

			Log.Info($"Killed by: {(d.From?.GetType().Name ?? "null")}");
			return true;
		}

		public override bool IgnoresProjectiles() {
			return GetComponent<StateComponent>().StateInstance is RollState;
		}

		private bool died;

		public override void AnimateDeath(DiedEvent d) {
			Dead = true;
			
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

			if (InGameState.Multiplayer) {
				stone.HasPlayer = true;
				stone.Index = GetComponent<InputComponent>().Index;
				stone.WasGamepad = GetComponent<InputComponent>().GamepadEnabled;

				if (GetComponent<InputComponent>().Index == 0) {
					var minIndex = 1024;
					Player pl = null;

					foreach (var p in Area.Tagged[Tags.Player]) {
						var i = p.GetComponent<InputComponent>().Index;

						if (p != this && i < minIndex) {
							minIndex = i;
							pl = (Player) p;
						}
					}

					if (pl != null) {
						var c = ForceGetComponent<ConsumablesComponent>();
						c.Entity = pl;
						Components.Remove(typeof(ConsumablesComponent));
						pl.Components[typeof(ConsumablesComponent)] = c;
						AddComponent(new ConsumablesComponent());
					}
				}
			}

			var pool = new List<string>();

			foreach (var i in GetComponent<InventoryComponent>().Items) {
				if (i.Type != ItemType.Hat && i.Id != "bk:no_lamp") {
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

			if (InGameState.EveryoneDied(this)) {
				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(stone, 0.5f);
			} else {
				((InGameState) Engine.Instance.State).ResetFollowing();
			}
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
			if (!Dead) {
				return;
			}

			var inventory = GetComponent<InventoryComponent>();

			foreach (var i in inventory.Items) {
				if (i.Id != "bk:no_lamp") {
					drops.Add(Items.Create(i.Id));
				}
			}

			foreach (var c in Components.Values) {
				if (c is ItemComponent i && i.Item != null && i.Item.Type != ItemType.Hat && i.Item.Id != "bk:no_lamp") {
					drops.Add(Items.Create(i.Item.Id));
				}
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (!GetComponent<HealthComponent>().Dead && (Run.LastDepth == -1 || Run.LastDepth == 0)) {
				var index = GetComponent<InputComponent>().Index;
				
				StartingWeapons[index] = GetComponent<ActiveWeaponComponent>().Item?.Id;
				StartingItems[index] = GetComponent<ActiveItemComponent>().Item?.Id;
				StartingLamps[index] = GetComponent<LampComponent>().Item?.Id;
			}
		}

		protected override string GetHurtSfx() {
			return null;
		}

		protected override string GetDeadSfx() {
			return null;
		}

		public T ForceGetComponent<T>() where T : Component {
			return base.GetComponent<T>();
		}

		public override T GetComponent<T>() {
			var minIndex = 1024;
			var pl = this;
			
			if (InGameState.Multiplayer && typeof(T) == typeof(ConsumablesComponent) && base.GetComponent<InputComponent>().Index != 0) {
				foreach (var p in Area.Tagged[Tags.Player]) {
					var i = p.GetComponent<InputComponent>().Index;
					
					if (i == 0) {
						return p.GetComponent<T>();
					}

					if (i < minIndex) {
						minIndex = i;
						pl = (Player) p;
					}
				}
			}
			
			return pl.ForceGetComponent<T>();
		}
	}
}