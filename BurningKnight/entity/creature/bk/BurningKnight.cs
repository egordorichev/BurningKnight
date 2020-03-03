using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.level.rooms;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
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

		public bool Hidden => GetComponent<StateComponent>().StateInstance is HiddenState;

		public override void AddComponents() {
			base.AddComponents();

			AddTag(Tags.BurningKnight);
			AddTag(Tags.PlayerSave);

			RemoveTag(Tags.Boss);
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
			health.InitMaxHealth = 10000;
			health.AutoKill = false;

			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());

			AddComponent(new LightComponent(this, 64, new Color(1f, 0.2f, 0.1f, 0.5f)));

			var buffs = GetComponent<BuffsComponent>();

			buffs.AddImmunity<FrozenBuff>();
			buffs.AddImmunity<BurningBuff>();

			Subscribe<RoomChangedEvent>();
			Subscribe<ItemTakenEvent>();
			Subscribe<Dialog.EndedEvent>();
			Subscribe<ShopNpc.SavedEvent>();
			Subscribe<ShopKeeper.EnragedEvent>();
			Subscribe<DiedEvent>();
			Subscribe<SecretRoomFoundEvent>();
			Subscribe<DefeatedEvent>();

			GetComponent<DialogComponent>().Dialog.Voice = 25;
		}

		public override void PostInit() {
			base.PostInit();
			
			GetComponent<AudioEmitterComponent>().Emit("mob_bk_hovering_loop", 0.3f, looped: true, tween: true);
			GetComponent<AudioEmitterComponent>().Emit("mob_bk_flame_loop",  0.3f, looped: true, tween: true);
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
					Become<ChaseState>();

					// YOU CAN'T DEFEAT THE BURNING KNIGHT!!!
					GetComponent<DialogComponent>().StartAndClose("bk_2", 5);
				}, 1f);
			};
		}

		public override bool HandleEvent(Event e) {
			if (e is DefeatedEvent bde) {
				if (bde.Boss == captured) {
					FreeSelf();
				}

				return false;
			}

			if (Hidden) {
				return base.HandleEvent(e);
			}

			if (e is RoomChangedEvent rce) {
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
			} else if (e is ItemTakenEvent ite) {
				if (ite.Stand is SingleChoiceStand) {
					GetComponent<DialogComponent>().StartAndClose("bk_1", 5);
					var state = GetComponent<StateComponent>();

					if (!(state.StateInstance is HiddenState)) {
						Timer.Add(() => {
							GetComponent<AudioEmitterComponent>().Emit("mob_bk_roar_1", 0.8f);
							state.Become<AttackState>();
						}, 3);
					}
				}
			} else if (e is Dialog.EndedEvent dse) {
				if (dse.Owner is ShopKeeper && dse.Dialog.Id == "shopkeeper_18") {
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
				}

				return false;
			} else if (e is SecretRoomFoundEvent) {
				// OH COMON, STOP EXPLODING MY CASTLE!
				GetComponent<DialogComponent>().StartAndClose("bk_8", 5);
			}

			return base.HandleEvent(e);
		}

		private float lastFadingParticle;

		public override void Update(float dt) {
			base.Update(dt);
			Done = false;

			if (Hidden) {
				return;
			}

			lastFadingParticle -= dt;

			if (lastFadingParticle <= 0) {
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

		private void CheckForScourgeRage() {
			var s = GetComponent<StateComponent>().StateInstance;

			if (s is ChaseState || s is AttackState || s is HiddenState) {
				return;
			}
			
			if (Run.Scourge >= 10) {
				Become<ChaseState>();
			}
		}

		private void CheckForScourgeRageFree() {
			var s = GetComponent<StateComponent>().StateInstance;

			if (s is IdleState || s is ChaseState || s is FollowState || s is HiddenState || s is FlyAwayAttackingState || s is AttackState) {
				return;
			}
			
			if (Run.Scourge < 10) {
				Become<FollowState>();
			}
		}

		#region Buring Knight States while calm
		public class IdleState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRage();
			}
		}

		public class FollowState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);
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
			var room = Target?.GetComponent<RoomComponent>()?.Room;

			if (room != null) {
				foreach (var mob in room.Tagged[Tags.Boss]) {
					if (mob != this && mob is Boss b) {
						captured = b;
						Become<CaptureState>();

						break;
					}
				}
			}
		}

		public class TeleportState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				Self.GetComponent<DialogComponent>().Close();
				var graphics = Self.GetComponent<BkGraphicsComponent>();

				Tween.To(0, graphics.Alpha, x => graphics.Alpha = x, 0.3f, Ease.QuadIn).OnEnd = () => {
					if (Self.Target != null) {
						Self.Center = Self.Target.Center + MathUtils.CreateVector(Rnd.AnglePI(), 64f);
					}

					Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f).OnEnd = () => {
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
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRageFree();

				var d = Self.DistanceTo(Self.Target);
				var force = 300f * dt;

				if (d < 64f) {
					Self.Become<FlyAwayAttackingState>();
				} else if (d <= 128f) {
					Self.Become<AttackState>();
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
			public override void Update(float dt) {
				base.Update(dt);
				Self.CheckForScourgeRageFree();

				if (Self.DistanceTo(Self.Target) < 64f) {
					Self.Become<FlyAwayAttackingState>();
				}

				if (T >= 1f) {
					var p = Projectile.Make(Self, "circle", Self.AngleTo(Self.Target) + Rnd.Float(-0.4f, 0.4f), 8, true, 0, null, 1);

					p.BreaksFromWalls = false;
					p.Spectral = true;
					p.Center = Self.Center;
					p.Depth = Self.Depth;

					Become<ChaseState>();
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
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_bk_capture");
					Camera.Instance.Unfollow(Self);

					Become<HiddenState>();
					Self.captured.SelectAttack();
				} else if (d >= 400f &&
				           Self.GetComponent<RoomComponent>().Room != Self.captured.GetComponent<RoomComponent>().Room) {
					Become<TeleportState>();

					return;
				}

				var a = Self.AngleTo(Self.captured);
				var force = 300f * dt;

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

				if (!teleported && Self.captured.Done) {
					teleported = true;
					Self.FreeSelf();
				}
			}
		}
	}
}