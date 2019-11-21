using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
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
			AddComponent(new LightComponent(this, 64f, new Color(1f, 0.7f, 0.2f, 1f)));

			var health = GetComponent<HealthComponent>();
			health.Unhittable = true;
			
			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());
			
			AddComponent(new LightComponent(this, 32, new Color(1f, 0.2f, 0.1f, 0.5f)));

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

		public override bool HandleEvent(Event e) {
			if (Hidden) {
				return base.HandleEvent(e);
			}
		
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New != null) {
					var t = rce.New.Type;

					if (t == RoomType.Boss) {
						foreach (var mob in rce.New.Tagged[Tags.MustBeKilled]) {
							if (mob != this && mob is Boss b) {
								captured = b;
								Become<CaptureState>();

								break;
							}
						}
					} else if (t == RoomType.Treasure) {
						foreach (var item in rce.New.Tagged[Tags.Item]) {
							if (item is SingleChoiceStand stand && stand.Item != null) {
								GetComponent<DialogComponent>().StartAndClose("bk_0", 5);
								break;
							}
						}
					} else if (t == RoomType.Granny) {
						// GRANNY, CAN YOU JUST DIE, PLEASE??
						GetComponent<DialogComponent>().StartAndClose("bk_9", 5);
					} else if (t == RoomType.OldMan) {
						// MY MASTER, I BROUGHT THE GOBLIN
						GetComponent<DialogComponent>().StartAndClose("bk_10", 5);
					}
				}
			} else if (e is ItemTakenEvent ite) {
				if (ite.Stand is SingleChoiceStand) {
					GetComponent<DialogComponent>().StartAndClose("bk_1", 5);
					var state = GetComponent<StateComponent>();

					if (!(state.StateInstance is HiddenState)) {
						Timer.Add(() => {
							Camera.Instance.Shake(10);
							state.Become<AttackState>();
						}, 3);
					}
				}
			} else if (e is Dialog.EndedEvent dse) {
				if (dse.Owner is ShopKeeper && dse.Dialog.Id == "shopkeeper_18") {
					// What a joke
					Timer.Add(() => {
						GetComponent<DialogComponent>().StartAndClose("bk_4", 5);
					}, 1);
				}
			} else if (e is ShopNpc.SavedEvent) {
				// I WOULDN'T BOTHER EVEN TALKING TO THEM
				GetComponent<DialogComponent>().StartAndClose("bk_5", 5);
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
			}
		}
		
		#region Buring Knight States while calm
		public class IdleState : SmartState<BurningKnight> {
			
		}
		
		public class FollowState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);
				var force = 200f * dt;

				if (d < 48f) {
					Self.Become<FlyAwayState>();
				} else if (d <= 72f) {
					return;
				} else if (d >= 200) {
					Self.Become<TeleportState>();
				}
				
				var a = Self.AngleTo(Self.Target);
				
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		
		public class FlyAwayState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);
				var force = -300f * dt;

				if (d > 80f) {
					Self.Become<FollowState>();
					return;
				}
				
				var a = Self.AngleTo(Self.Target);
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
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
		}
		#endregion
		
		#region Burning Knight States while chasing
		public class FlyAwayAttackingState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);
				var force = -300f * dt;

				if (d > 96f) {
					Self.Become<ChaseState>();
					return;
				}
				
				var a = Self.AngleTo(Self.Target);
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		
		public class ChaseState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);
				
				var d = Self.DistanceTo(Self.Target);
				var force = 300f * dt;

				if (d < 64f) {
					Self.Become<FlyAwayAttackingState>();
				} else if (d <= 128f) {
					Self.Become<AttackState>();
				}

				var a = Self.AngleTo(Self.Target);

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class AttackState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.DistanceTo(Self.Target) < 64f) {
					Self.Become<FlyAwayAttackingState>();
				}

				if (T >= 0.5f) {
					var p = Projectile.Make(Self, "big", Self.AngleTo(Self.Target) + Rnd.Float(-0.2f, 0.2f), 10, true, 0, null, 1);

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
				
				Camera.Instance.Follow(Self, 0.3f);
				
				Timer.Add(() => {
					Camera.Instance.Follow(Self.captured, 0.3f);
				}, 0.5f);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var d = Self.DistanceTo(Self.captured);
				
				if (d <= 8) {
					// PREPARE TO DIE!
					Self.captured.GetComponent<DialogComponent>().StartAndClose("bk_3", 5);
					Camera.Instance.Unfollow(Self);
					
					Become<HiddenState>();
					Self.captured.SelectAttack();
				}
				
				var a = Self.AngleTo(Self.captured);
				var force = 300f * dt;

				if (d <= 64f) {
					force *= 2;
				}

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
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
					
					var graphics = Self.GetComponent<BkGraphicsComponent>();
					graphics.Alpha = 0;

					Self.Center = Self.captured.Center;
				
					Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f).OnEnd = () => {
						Timer.Add(() => {
							Self.Become<ChaseState>();
							// YOU CAN'T DEFEAT THE BURNING KNIGHT!!!
							Self.GetComponent<DialogComponent>().StartAndClose("bk_2", 5);
						}, 1f);
					};
				}
			}
		}
	}
}