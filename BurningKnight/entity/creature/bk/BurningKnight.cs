using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.cutscene.entity;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.level;
using BurningKnight.level.biome;
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
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using VelcroPhysics.Dynamics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private BossPatternSet<BurningKnight> set;
		private static Color tint = new Color(234, 50, 60, 200);
		private Boss captured;
		private bool raging;
		private int timesRaged;

		public bool Passive;
		public bool Hidden => GetComponent<StateComponent>().StateInstance is HiddenState;

		public override void AddComponents() {
			base.AddComponents();

			AddTag(Tags.BurningKnight);
			AddTag(Tags.PlayerSave);

			RemoveTag(Tags.Boss);
			RemoveTag(Tags.Mob);
			RemoveTag(Tags.LevelSave);
			RemoveTag(Tags.MustBeKilled);

			Width = 22;
			Height = 27;
			Flying = true;
			TargetEverywhere = true;
			AlwaysActive = true;
			AlwaysVisible = true;
			HasHealthbar = false;
			Depth = Layers.Bk;
			TouchDamage = 0;
			
			var b = new RectBodyComponent(6, 8, 10, 15, BodyType.Dynamic, true) {
				KnockbackModifier = 0
			};

			AddComponent(b);
			b.Body.LinearDamping = 3;

			AddComponent(new BkGraphicsComponent("old_burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.Unhittable = true;
			health.InitMaxHealth = 500;
			// health.AutoKill = false;

			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());

			AddComponent(new LightComponent(this, 64, new Color(1f, 0.2f, 0.1f, 0.5f)));

			var buffs = GetComponent<BuffsComponent>();

			buffs.AddImmunity<FrozenBuff>();
			buffs.AddImmunity<BurningBuff>();
			buffs.AddImmunity<BleedingBuff>();

			Subscribe<RoomChangedEvent>();
			Subscribe<ItemTakenEvent>();
			Subscribe<Dialog.EndedEvent>();
			Subscribe<ShopNpc.SavedEvent>();
			Subscribe<ShopKeeper.EnragedEvent>();
			Subscribe<DiedEvent>();
			Subscribe<SecretRoomFoundEvent>();
			Subscribe<DefeatedEvent>();
			Subscribe<NewLevelStartedEvent>();

			GetComponent<DialogComponent>().Dialog.Voice = 25;
			AddComponent(new AimComponent(AimComponent.AimType.Target));
		}

		public override void PostInit() {
			base.PostInit();
			
			Timer.Add(() => {
				GetComponent<AudioEmitterComponent>().Emit("mob_bk_hovering_loop", 0.3f, looped: true, tween: true);
				GetComponent<AudioEmitterComponent>().Emit(Run.Depth == 10 ? "mob_bk_fight_loop" : "mob_bk_flame_loop",  0.3f, looped: true, tween: true);
			}, 2f);
		}

		public override void Destroy() {
			base.Destroy();
			
			GetComponent<AudioEmitterComponent>().StopAll();
			Log.Debug("Bk destroyed");
		}

		protected override void OnTargetChange(Entity target) {
			if (Hidden) {
				return;
			}

			if (!Awoken && target != null) {
				Awoken = true;
				Become<FollowState>();
			} else if (target == null) {
				Become<IdleState>();
				Awoken = false;
			}

			base.OnTargetChange(target);
		}

		private void FreeSelf() {
			if (!Hidden) {
				return;
			}

			var graphics = GetComponent<BkGraphicsComponent>();
			graphics.Alpha = 0;

			Center = captured.Center;
			GetComponent<HealthComponent>().Unhittable = true;

			Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f).OnEnd = () => {
				GetComponent<AudioEmitterComponent>().Emit("mob_bk_roar_4", 0.8f);
									
				Timer.Add(() => {
					if (captured.Done) {
						Become<ChaseState>();

						// YOU CAN'T DEFEAT THE BURNING KNIGHT!!!
						GetComponent<DialogComponent>().StartAndClose("bk_2", 5);
					} else {
						captured = null;
						Become<FollowState>();
						CheckForScourgeRage();
					}
				}, 1f);
			};
		}

		public override bool HandleEvent(Event e) {
			if (e is DefeatedEvent bde) {
				if (bde.Boss == captured) {
					FreeSelf();
				}

				if (bde.Boss == this) {
					return base.HandleEvent(e);
				}

				return false;
			}

			if (Hidden) {
				return base.HandleEvent(e);
			}

			if (e is RoomChangedEvent rce) {
				if (!InFight) {
					var p = rce.Who is Player;
					var bs = rce.Who is BurningKnight;

					if ((p || bs) && rce.New != null) {
						var t = rce.New.Type;

						if (t == RoomType.Boss) {
							CheckCapture();
						} else if (p) {
							if (t == RoomType.Treasure) {
								foreach (var item in rce.New.Tagged[Tags.Item]) {
									if (item is SingleChoiceStand stand && stand.Item != null) {
										GetComponent<DialogComponent>().StartAndClose("bk_0", 5);

										break;
									}
								}
							} else if (t == RoomType.Granny) {
								// GRANNY, CAN YOU JUST DIE, PLEASE??
								GetComponent<DialogComponent>().StartAndClose("bk_9", 3);
							} else if (t == RoomType.OldMan) {
								// MY MASTER, I BROUGHT THE GOBLIN
								GetComponent<DialogComponent>().StartAndClose("bk_10", 5);
							}
						}
					}
				}
			} else if (e is ItemTakenEvent ite) {
				if (!InFight && ite.Stand is SingleChoiceStand && ite.Who is Player) {
					GetComponent<DialogComponent>().StartAndClose("bk_1", 5);
					var state = GetComponent<StateComponent>();

					if (!(state.StateInstance is HiddenState)) {
						Timer.Add(() => {
							timesRaged++;
							GetComponent<AudioEmitterComponent>().Emit("mob_bk_roar_1", 0.8f);
							state.Become<AttackState>();
						}, 3);
					}
				}
			} else if (e is Dialog.EndedEvent dse) {
				if (!InFight && dse.Owner is ShopKeeper && dse.Dialog.Id == "shopkeeper_18") {
					// What a joke
					Timer.Add(() => { GetComponent<DialogComponent>().StartAndClose("bk_4", 5); }, 1);
				}
			} else if (e is ShopNpc.SavedEvent) {
				// I WOULDN'T BOTHER EVEN TALKING TO THEM
				Timer.Add(() => { GetComponent<DialogComponent>().StartAndClose("bk_5", 5); }, 2f);
			} else if (e is ShopKeeper.EnragedEvent skee) {
				if (skee.ShopKeeper.GetComponent<RoomComponent>().Room.Explored) {
					// KILL HIM, EDWARD!
					GetComponent<DialogComponent>().StartAndClose("bk_6", 5);
				}
			} else if (e is DiedEvent de) {
				if (de.Who is ShopKeeper) {
					// EDWARD, NOOOOOO!
					GetComponent<DialogComponent>().StartAndClose("bk_7", 5);
					return false;
				} else if (de.Who == this) {
					died = true;
					return base.HandleEvent(e);
				} else {
					return false;
				}
			} else if (e is SecretRoomFoundEvent) {
				// OH COMON, STOP EXPLODING MY CASTLE!
				GetComponent<DialogComponent>().StartAndClose("bk_8", 5);
			} else if (e is NewLevelStartedEvent) {
				if (!InFight) {
					CheckForScourgeRage();
					var state = GetComponent<StateComponent>().StateInstance;

					if (raging && !(state is AttackState || state is ChaseState || state is FlyAwayAttackingState)) {
						raging = false;
						sayNoRage = true;
					}
				}
			}

			return base.HandleEvent(e);
		}

		private bool died;
		
		private bool sayNoRage;

		public override void Load(FileReader stream) {
			base.Load(stream);
			raging = stream.ReadBoolean();
			timesRaged = stream.ReadInt32();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(raging);
			stream.WriteInt32(timesRaged);
		}

		private float lastFadingParticle;

		public override void Update(float dt) {
			base.Update(dt);

			if (!Placed) {
				Done = false;
			}

			if (Hidden) {
				return;
			}

			if (lasers.Count > 0) {
				spinV += dt;

				foreach (var l in lasers) {
					if (l.Done) {
						lasers.Clear();
						break;
					}

					l.Angle += spinV * dt * 0.2f * spinDir;
				}
			}

			lastFadingParticle -= dt;

			if (lastFadingParticle <= 0 && !(GetComponent<StateComponent>().StateInstance is FlameAttack)) {
				lastFadingParticle = 0.2f;

				var particle = new FadingParticle(GetComponent<BkGraphicsComponent>().Animation.GetCurrentTexture(), tint);
				Area.Add(particle);

				particle.Depth = Depth - 1;
				particle.Center = Center;

				var room = GetComponent<RoomComponent>().Room;

				if (room != null && room.Type == RoomType.Boss) {
					CheckCapture();
				} else if (Target != null) {
					room = Target.GetComponent<RoomComponent>().Room;

					if (room != null && room.Type == RoomType.Boss) {
						CheckCapture();
					}
				}
			}
		}

		public bool ForcedRage;

		public void CheckForScourgeRage() {
			if (InFight) {
				return;
			}
			
			var s = GetComponent<StateComponent>().StateInstance;

			if (s is ChaseState || s is AttackState || s is HiddenState) {
				return;
			}
			
			if (ForcedRage || Run.Scourge >= 10) {
				Become<ChaseState>();
			}
		}

		private void CheckForScourgeRageFree() {
			if (InFight) {
				return;
			}
			
			if (Target == null) {
				Become<IdleState>();
			}
			
			var s = GetComponent<StateComponent>().StateInstance;

			if (s is IdleState || s is ChaseState || s is FollowState || s is HiddenState || s is FlyAwayAttackingState || s is AttackState) {
				return;
			}
			
			if (!ForcedRage && Run.Scourge < 10) {
				Become<FollowState>();
			}
		}

		#region Buring Knight States while calm
		public class IdleState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					Self.CheckForScourgeRage();
				}
			}
		}

		public class FollowState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Become<IdleState>();
					return;
				}
				
				Self.CheckForScourgeRage();

				var d = Self.DistanceTo(Self.Target);
				var force = 200f * dt;

				if (d < 48f) {
					Self.Become<FlyAwayState>();
				} else if (d <= 72f) {
					return;
				} else if (d >= 300) {
					Self.Become<TeleportState>();
				}

				var room = Self.Target.GetComponent<RoomComponent>().Room;

				if (Self.OnScreen && room != null && room.Type == RoomType.Regular &&
				    room.Tagged[Tags.MustBeKilled].Count > 0 && room.Contains(Self, 16f)) {
					var aa = Self.AngleTo(room);
					force = 400f * dt;

					Self.GetComponent<RectBodyComponent>().Velocity -=
						new Vector2((float) Math.Cos(aa) * force, (float) Math.Sin(aa) * force);

					return;
				}

				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity +=
					new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class FlyAwayState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRage();

				var d = Self.DistanceTo(Self.Target);
				var force = -300f * dt;

				if (d > 80f) {
					Self.Become<FollowState>();

					return;
				}

				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity +=
					new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		private void CheckCapture() {
			if (InFight) {
				return;
			}
			
			var room = Target?.GetComponent<RoomComponent>()?.Room;

			if (room != null && room.Type == RoomType.Boss) {
				foreach (var p in room.Tagged[Tags.Player]) {
					if (!((Player) p).Teleported) {
						return;
					}
				}

				if (Run.Level.Biome is LibraryBiome) {
					BeginFight();
					return;
				}
			
				foreach (var mob in room.Tagged[Tags.Boss]) {
					if (mob != this && mob is Boss b && !(b is bk.BurningKnight)) {
						captured = b;
						Become<CaptureState>();

						break;
					}
				}
			}
		}

		public class CutsceneState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				var bkDialog = Self.GetComponent<DialogComponent>();
				var playerDialog = Self.Target.GetComponent<DialogComponent>();
				
				Start(bkDialog, "bkw_0", Self.Target, () => {
					Start(bkDialog, "bkw_1", Self.Target, () => {
						bkDialog.Close();
						
						Start(playerDialog, "bkw_2", Self.Target, () => {
							playerDialog.Close();
							
							Start(bkDialog, "bkw_3", Self.Target, () => {
								Become<FollowState>();
								bkDialog.OnEnd();
								GlobalSave.Put("bk_who", true);

								Self.Target.GetComponent<HealthComponent>().Unhittable = false;
							});	
						});	
					});	
				});
			}
		
			private void Start(DialogComponent d, string id, Entity to, Action callback = null) {
				d.Start(id, to);

				if (callback != null) {
					d.Dialog.ShowArrow = true;
					d.Dialog.OnEnd = () => {
						Timer.Add(callback, 0.1f);
						return true;
					};
				}
			}
		}

		public class TeleportState : SmartState<BurningKnight> {
			private bool did;

			public override void Update(float dt) {
				base.Update(dt);

				if (did) {
					return;
				}

				did = true;
				
				Self.GetComponent<DialogComponent>().Close();
				var graphics = Self.GetComponent<BkGraphicsComponent>();

				Tween.To(0, graphics.Alpha, x => graphics.Alpha = x, 0.3f, Ease.QuadIn).OnEnd = () => {
					if (Self.Target != null) {
						Self.Center = Self.Target.Center + MathUtils.CreateVector(Rnd.AnglePI(), 64f);
					}

					Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f).OnEnd = () => {
						if (Run.Depth == 1 && GlobalSave.IsFalse("bk_who")) {
							Self.Become<CutsceneState>();
							return;
						}
						
						if (Self.sayNoRage) {
							Self.sayNoRage = false;
							Self.GetComponent<DialogComponent>().StartAndClose("bk_11", 5);
						}

						if (Self.Target != null) {
							Self.Become<FollowState>();
						} else {
							Self.Become<IdleState>();
						}
					};
				};
			}

			public override void Destroy() {
				base.Destroy();
				Self.CheckCapture();
			}
		}
		#endregion

		#region Burning Knight States while chasing
		public class FlyAwayAttackingState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();
				Self.raging = true;
			}

			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRageFree();

				var d = Self.DistanceTo(Self.Target);
				var force = -300f * dt;

				if (d > 96f) {
					Self.Become<ChaseState>();

					return;
				}

				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity +=
					new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class ChaseState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();
				Self.raging = true;
			}
			
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRageFree();

				var d = Self.DistanceTo(Self.Target);
				var force = 300f * dt;

				if (d < 64f) {
					Self.Become<FlyAwayAttackingState>();
				} else if (d <= 128f) {
					var r = Self.Target.GetComponent<RoomComponent>().Room;

					if (r.Type == RoomType.Shop || r.Type == RoomType.SubShop || r.Type == RoomType.OldMan) {

					} else {
						Self.Become<AttackState>();
					}
				}

				var room = Self.Target.GetComponent<RoomComponent>().Room;

				if (Self.OnScreen && room != null && room.Type == RoomType.Regular &&
				    room.Tagged[Tags.MustBeKilled].Count > 0 && room.Contains(Self, 16f)) {
					var aa = Self.AngleTo(room);
					force = 400f * dt;

					Self.GetComponent<RectBodyComponent>().Velocity -=
						new Vector2((float) Math.Cos(aa) * force, (float) Math.Sin(aa) * force);

					return;
				}

				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity +=
					new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class AttackState : SmartState<BurningKnight> {
			private int count;
			
			public override void Init() {
				base.Init();
				Self.raging = true;
				count = Math.Min(1, Self.timesRaged);
			}
			
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRageFree();

				if (Self.DistanceTo(Self.Target) < 64f) {
					Self.Become<FlyAwayAttackingState>();
					return;
				}

				var r = Self.Target.GetComponent<RoomComponent>().Room;

				if (r.Type == RoomType.Shop || r.Type == RoomType.SubShop || r.Type == RoomType.OldMan) {
					Self.Become<ChaseState>();
					return;
				}

				if (T >= 1f) {
					Self.GetComponent<AudioEmitterComponent>().Emit("mob_bk_fire");

					var c = 1;

					if (Self.timesRaged > 2 && Self.timesRaged < 5) {
						c = 3;
					}

					for (var i = 0; i < c; i++) {
						var p = Projectile.Make(Self, "circle", Self.AngleTo(Self.Target) + Rnd.Float(-0.4f, 0.4f) + (c == 1 ? 0 : (i - 1) * Math.PI * 0.2f), 8 + Self.timesRaged * 0.3f, true, 0, null, Rnd.Float(1f, 1f + Self.timesRaged * 0.1f));

						p.BreaksFromWalls = false;
						p.Spectral = true;
						p.Center = Self.Center;
						p.Depth = Self.Depth;
						p.CanBeReflected = false;
						p.CanBeBroken = false;
						p.Color = ProjectileColor.BkRed;

						if (Self.timesRaged > 4) {
							p.Controller += TargetProjectileController.Make(Self.Target, 0.5f);
							p.Range = 5f + Rnd.Float(1f);
						}
					}
					
					count--;
					T -= 0.25f + (Self.timesRaged - 1) * 0.1f;

					if (count <= 0) {
						Become<ChaseState>();
					}
				}
			}
		}

		public override void SelectAttack() {
			if (set == null) {
				set = BurningKnightAttackRegistry.PatternSetRegistry.Generate(Run.Level.Biome.Id);
			}

			base.SelectAttack();
			GetComponent<StateComponent>().PushState(BurningKnightAttackRegistry.GetNext(set));
		}
		#endregion

		public class CaptureState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(Self, 0.3f);

				Timer.Add(() => { Camera.Instance.Follow(Self.captured, 0.3f); }, 0.5f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.captured);

				if (d <= 8) {
					Audio.PlayMusic("Fatiga", true);

					// PREPARE TO DIE!
					Self.captured.GetComponent<DialogComponent>().StartAndClose(Self.captured.GetScream(), 5);
					Self.captured.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_bk_capture");
					Camera.Instance.Unfollow(Self);

					Become<HiddenState>();
					Self.captured.SelectAttack();
				} else if (d >= 400f &&
				           Self.GetComponent<RoomComponent>().Room != Self.captured.GetComponent<RoomComponent>().Room) {
					Become<TeleportState>();

					return;
				}

				var a = Self.AngleTo(Self.captured);
				var force = 500f * dt;

				if (d <= 64f) {
					force *= 2;
				}

				Self.GetComponent<RectBodyComponent>().Velocity +=
					new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class HiddenState : SmartState<BurningKnight> {
			private bool teleported;

			public override void Init() {
				base.Init();

				Camera.Instance.Shake(20);
				Self.Position = Vector2.Zero;

				Timer.Add(() => {
					((InGameState) Engine.Instance.State).ResetFollowing();
					Camera.Instance.Shake(10);
				}, 1);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!teleported && (Self.captured.Done || Self.captured.GetComponent<StateComponent>().StateInstance is FriendlyState)) {
					teleported = true;
					Self.FreeSelf();
				}
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.Checkbox("Raging", ref raging)) {
				if (raging) {
					Become<AttackState>();
				} else {
					Become<FollowState>();
				}
			}
			
			ImGui.InputInt("Times Raged", ref timesRaged);
		}

		public override void PlaceRewards() {
			var head = new BkHead();
			Area.Add(head);
			head.Center = Center;
			Audio.PlayMusic("Last chance");
		}

		protected override void CreateGore(DiedEvent d) {
			
		}

		public bool InFight;

		private void AddOrbitals(int count) {
			for (var i = 0; i < count; i++) {
				var orbital = new BkOrbital {
					Id = i
				};
				
				Area.Add(orbital);
				orbital.Center = Center;
				GetComponent<OrbitGiverComponent>().AddOrbiter(orbital);
			}
		}
		
		private void BeginFight() {
			if (InFight || Passive) {
				return;
			}
			
			var r = GetComponent<RoomComponent>().Room;

			if (r == null) {
				return;
			}
			
			GetComponent<DialogComponent>().StartAndClose("bk_12", 3);
			AddOrbitals(6);

			InFight = true;
			HasHealthbar = true;

			if (HealthBar == null) {
				HealthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(HealthBar);
				AddPhases();
			}
			
			AddTag(Tags.Boss);
			AddTag(Tags.Mob);
			AddTag(Tags.MustBeKilled);

			var a = r.Tagged[Tags.MustBeKilled];

			if (!a.Contains(this)) {
				a.Add(this);
			}
			
			a = r.Tagged[Tags.Mob];

			if (!a.Contains(this)) {
				a.Add(this);
			}
			
			a = r.Tagged[Tags.Boss];

			if (!a.Contains(this)) {
				a.Add(this);
			}

			Become<FightState>();

			GetComponent<HealthComponent>().Unhittable = false;
			TouchDamage = 2;
			Center = Target.GetComponent<RoomComponent>().Room.Center;
		}

		protected override void Become<T>() {
			if (!Passive || typeof(T) == typeof(IdleState)) {
				base.Become<T>();
			}
		}

		protected override void AddPhases() {
			HealthBar.AddPhase(0.5f);
		}
		
		/*
		 * The actual boss battle
		 */

		private int count;

		public class FightState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 1f) {
					switch (Self.count) {
						case 0: {
							Become<LaserSwingAttack>();
							break;
						}
						
						case 1: {
							Become<SpawnAttack>();
							break;
						}

						case 2: {
							Become<LaserRotateAttack>();
							break;
						}
						
						case 3: {
							Become<FlameAttack>();
							break;
						}
						
						case 4: {
							Become<SwordAttackState>();
							break;
						}
						
						case 5: {
							Become<SkullAttack>();
							break;
						}
						
						case 6: {
							Become<LaserCageAttack>();
							break;
						}
					}
					
					Self.count = (Self.count + 1) % (Self.Raging ? 7 : 6);
				}
			}
		}
		
		public bool Raging => GetComponent<HealthComponent>().Percent <= 0.5f;

		public class LaserSwingAttack : SmartState<BurningKnight> {
			private Laser laser;
			private float vy;
			private float angle;
			
			public override void Init() {
				base.Init();

				angle = Self.AngleTo(Self.Target) - (Rnd.Chance() ? -1 : 1) * 1.2f;
				Self.WarnLaser(angle);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (laser == null) {
					if (T < 1f) {
						return;
					}

					if (Self.Raging) {
						Self.StartLasers();
					}
					
					laser = Laser.Make(Self, 0, 0, damage: 2, scale: 3, range: 64);
					laser.LifeTime = 10f;
					laser.Position = Self.Center;
					laser.Angle = angle;
					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);
				}

				if (laser.Done) {
					Become<FightState>();
					return;
				}
				
				laser.Position = Self.Center;

				var aa = laser.Angle;
				var a = Self.AngleTo(Self.Target);
				
				vy += (float) MathUtils.ShortAngleDistance(aa, a) * dt * 4;

				laser.Angle += vy * dt * 0.5f;
			}
		}

		private List<Laser> lasers = new List<Laser>();
		private float spinV;
		private int spinDir;

		private void WarnLaser(float angle, Vector2? offset = null) {
			for (var i = 0; i < 3; i++) {
				Timer.Add(() => {
					var projectile = Projectile.Make(this, Raging ? "big" : "circle", angle, Raging ? 15f : 10f);

					projectile.AddLight(32f, Projectile.RedLight);
					projectile.Center += MathUtils.CreateVector(angle, 8);

					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					if (offset != null) {
						projectile.Center += offset.Value;
					}
				}, i * 0.3f);
			}		
		}

		private void StartLasers() {
			lasers.Clear();
			spinV = 0;
			spinDir = Rnd.Chance() ? 1 : -1;

			Timer.Add(() => { 
				GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);
			}, 1f);

			for (var i = 0; i < 4; i++) {
				var angle = AngleTo(Target) + (i / 4f + 1 / 8f) * (float) Math.PI * 2f;

				WarnLaser(angle);

				Timer.Add(() => {
					var laser = Laser.Make(this, 0, 0, damage: 2, scale: 3, range: 64);
					laser.LifeTime = 10f;
					laser.Position = Center;
					laser.Angle = angle;

					lasers.Add(laser);
				}, 1f);
			}
		}

		public class LaserRotateAttack : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();
				Self.StartLasers();
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 3f && Self.lasers.Count == 0) {
					Become<FightState>();
				}
			}
		}
		
		public class SkullAttack : SmartState<BurningKnight> {
			private int count;
			private bool explode;

			public override void Init() {
				base.Init();
				explode = Rnd.Chance();
			}

			public override void Update(float dt) {
				base.Update(dt);

				if ((count + 1) * (Self.Raging ? 0.7f : 1f) <= T) {
					count++;

					if (Self.Target == null || Self.Died) {
						return;
					}
					
					var a = Self.GetComponent<BkGraphicsComponent>();
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_oldking_shoot");

					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.3f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.3f);

						if (Self.Target == null || Self.Died) {
							return;
						}

						var skull = Projectile.Make(Self, explode ? "skull" : "skup", Rnd.AnglePI(), explode ? Rnd.Float(5, 12) : 14);

						skull.CanBeReflected = false;
						skull.CanBeBroken = false;
						
						if (explode) {
							skull.NearDeath += p => {
								var c = new AudioEmitterComponent {
									DestroySounds = false
								};
								
								p.AddComponent(c);
								c.Emit("mob_oldking_explode");
							};
						
							skull.OnDeath += (p, e, t) => {
								if (!t) {
									return;
								}
						
								for (var i = 0; i < 16; i++) {
									var bullet = Projectile.Make(Self, "small", 
										((float) i) / 8 * (float) Math.PI, (i % 2 == 0 ? 2 : 1) * 4 + 3);

									bullet.CanBeReflected = false;
									bullet.CanBeBroken = false;
									bullet.Center = p.Center;
								}
							};
						}

						skull.Controller += TargetProjectileController.Make(Self.Target, 0.5f);
						skull.Range = 5f;
						skull.IndicateDeath = true;
						skull.CanBeReflected = false;
						skull.GetComponent<ProjectileGraphicsComponent>().IgnoreRotation = true;
						
						if (count == (Self.Raging ? 6 : 4)) {
							Self.Become<FightState>();
						}
					};
				}
			}
		}

		private static string[] swordData = {
			" x     ",
			"xxxxxxx",
			" x     ",
		};

		public class SwordAttackState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				Timer.Add(() => {
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire_static");

					var a = Self.AngleTo(Self.Target);
					
					var p = new ProjectilePattern(KeepShapePattern.Make(0)) {
						Position = Self.Center
					};

					Self.Area.Add(p);
					
					ProjectileTemplate.MakeFast(Self, "small", Self.Center, a, (pr) => {
						pr.CanBeReflected = false;
						pr.CanBeBroken = false;
						
						p.Add(pr);
						pr.Color = ProjectileColor.Red;
						pr.AddLight(32, pr.Color);
					}, swordData, () => {
						Timer.Add(() => {
							p.Launch(a, 30);
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire_static");

							Become<FightState>();
						}, 0.2f);
					});
				}, 1f);
			}
		}

		public class LaserCageAttack : SmartState<BurningKnight> {
			private Laser[] lasers = new Laser[8];
			private Vector2 spot;

			private const float boxHalfSize = 32;

			private static Vector2[] laserOffsets = {
				new Vector2(-boxHalfSize, 0), 
				new Vector2(-boxHalfSize, 0), 
				new Vector2(boxHalfSize, 0),
				new Vector2(boxHalfSize, 0),
				new Vector2(0, -boxHalfSize),
				new Vector2(0, -boxHalfSize),
				new Vector2(0, boxHalfSize),
				new Vector2(0, boxHalfSize),
			};

			private static double[] laserAngles = {
				Math.PI * 0.5f,
				Math.PI * 1.5f,
				Math.PI * 0.5f,
				Math.PI * 1.5f,
				Math.PI,
				0,
				Math.PI,
				0
			};

			public override void Init() {
				base.Init();

				spot = Self.Center;

				for (var i = 0; i < 8; i++) {
					Self.WarnLaser((float) laserAngles[i], laserOffsets[i]);
				}
				
				Timer.Add(() => {
					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);
					
					for (var i = 0; i < 8; i++) {
						var laser = Laser.Make(Self, 0, 0, damage: 2, scale: 3, range: 64);
						laser.LifeTime = 10f;
						laser.Position = spot + laserOffsets[i];
						laser.Angle = (float) laserAngles[i];
						lasers[i] = laser;
					}
				}, 1);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T < 1f) {
					return;
				}
				
				spot = spot.Lerp(Self.Target.Center, dt * 0.5f);

				for (var i = 0; i < 8; i++) {
					var laser = lasers[i];

					if (laser.Done) {
						foreach (var l in lasers) {
							l.Done = true;
						}
						
						Become<FightState>();
						return;
					}
					
					laser.Position = spot + laserOffsets[i];
				}
			}
		}

		public class FlameAttack : SmartState<BurningKnight> {
			private float r;
			private List<int> last = new List<int>();
			
			public override void Init() {
				base.Init();
				
				var graphics = Self.GetComponent<BkGraphicsComponent>();
				Self.GetComponent<HealthComponent>().Unhittable = true;
				Tween.To(0, graphics.Alpha, x => graphics.Alpha = x, 0.3f);
				Self.TouchDamage = 0;
			}

			public override void Destroy() {
				base.Destroy();
				
				var graphics = Self.GetComponent<BkGraphicsComponent>();
				Self.GetComponent<HealthComponent>().Unhittable = false;
				Self.TouchDamage = 2;
				Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f);

				foreach (var l in last) {
					Run.Level.SetFlag(l, Flag.Burning, false);
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 10f) {
					Become<FightState>();
					return;
				}
				
				r = Math.Min(1.5f, r + dt * 60);
				
				var x = (int) Math.Floor(Self.CenterX / 16);
				var y = (int) Math.Floor(Self.CenterY / 16);

				for (var xx = (int) -r; xx <= r; xx++) {
					for (var yy = (int) -r; yy <= r; yy++) {
						if (Math.Sqrt(xx * xx + yy * yy) <= r) {
							var i = Run.Level.ToIndex(x + xx, y + yy);

							if (!Run.Level.CheckFlag(i, Flag.Burning)) {
								Run.Level.SetFlag(i, Flag.Burning, true);
								last.Add(i);

								Timer.Add(() => {
									last.Remove(i);
									Run.Level.SetFlag(i, Flag.Burning, false);
								}, Rnd.Float(1.5f, 2.5f));
							}
						}
					}
				}
				
				var force = 250f * dt;
				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		
		public class SpawnAttack : SmartState<BurningKnight> {
			private int count;
			private float delay;

			public override void Init() {
				base.Init();
				count = Rnd.Int(4, 10);
			}

			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;

				if (delay <= 0) {
					delay = 0.3f;
					Self.GetComponent<BkGraphicsComponent>().Animate();

					var angle = Rnd.AnglePI() * 0.5f + count * (float) Math.PI;
					var projectile = Projectile.Make(Self, "big", angle, 15f);

					projectile.Color = ProjectileColor.Orange;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(angle, 8);

					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					projectile.OnDeath += (p, en, t) => {
						var x = (int) Math.Floor(p.CenterX / 16);
						var y = (int) Math.Floor(p.CenterY / 16);
						
						var mob = new WallCrawler();
						Self.Area.Add(mob);
						mob.X = x * 16;
						mob.Y = y * 16 - 8;
						mob.GeneratePrefix();
						AnimationUtil.Poof(mob.Center, 1);
					};

					count--;

					if (count <= 0) {
						Become<FightState>();
					}
				}
			}
		}
	}
}