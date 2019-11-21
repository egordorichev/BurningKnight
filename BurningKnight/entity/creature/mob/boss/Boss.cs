using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
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
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public bool Awoken;
		
		protected bool HasHealthbar = true;
		protected HealthBar HealthBar;

		private bool died;
		private float deathTimer;
		private float lastExplosion;

		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new DialogComponent());
			
			GetComponent<BuffsComponent>().AddImmunity<CharmedBuff>();
			Become<FriendlyState>();
			
			AddTag(Tags.Boss);
		}
		
		private bool cleared;

		public override void Update(float dt) {
			base.Update(dt);
			
			if (died) {
				if (!cleared) {
					cleared = true;
					GetComponent<DialogComponent>().Close();
					
					foreach (var p in Area.Tagged[Tags.Projectile]) {
						AnimationUtil.Poof(p.Center);
						((Projectile) p).Break();
					}
				}

				if (deathTimer >= 3f) {
					HandleEvent(new DefeatedEvent {
						Boss = this
					});

					var player = LocalPlayer.Locate(Area);

					// fixme: do this only if a deal was open and only to the deals open
					GetComponent<RoomComponent>().Room.PaintTunnel(Tile.FloorD);
					Run.Level.TileUp();
					Run.Level.CreateBody();
					
					if (player != null) {
						var stats = player.GetComponent<StatsComponent>();

						if (stats.SawDeal && !stats.TookDeal && Rnd.Chance(stats.GrannyChance * 100)) {
							foreach (var r in Area.Tagged[Tags.Room]) {
								var room = (Room) r;

								if (room.Type == RoomType.Granny) {
									room.OpenHiddenDoors();
									break;
								}
							}
						}
						
						if (Rnd.Chance(stats.DMChance * 100)) {
							foreach (var r in Area.Tagged[Tags.Room]) {
								var room = (Room) r;

								if (room.Type == RoomType.OldMan) {
									room.OpenHiddenDoors();
									break;
								}
							}
						}
					}
					
					Done = true;
					PlaceRewards();
					
					Timer.Add(() => {
						((InGameState) Engine.Instance.State).ResetFollowing();
					}, 0.5f);
				} else {
					deathTimer += dt;
					lastExplosion -= dt;
				
					if (lastExplosion <= 0) {
						lastExplosion = 0.3f;
						AnimationUtil.Explosion(Center + new Vector2(Rnd.Float(-16, 16), Rnd.Float(-16, 16)));
						Camera.Instance.Shake(10);
						Audio.PlaySfx("explosion");
					}

					if (deathTimer > 2.5f) {
						Engine.Instance.FlashColor = new Color(1f, 1f, 1f, (deathTimer - 2.5f) * 2f);
						Engine.Instance.Flash = 0.01f;
					}
				}

				return;
			}
			
			if (!(GetComponent<StateComponent>().StateInstance is FriendlyState) && HasHealthbar && HealthBar == null) {
				HealthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(HealthBar);
			}
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Awoken = false;
			} else {
				Awoken = true;
			}
			
			base.OnTargetChange(target);
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				if (!died) {
					died = true;
					HealthBar?.Remove();

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 1f);
					Become<DefeatedState>();

					Audio.Stop();
				}
				
				Done = false;
				e.Handled = true;
			}

			return base.HandleEvent(e);
		}

		public void PlaceRewards() {
			var exit = new Exit();
			Area.Add(exit);
				
			exit.To = Run.Depth + 1;

			var center = GetComponent<RoomComponent>().Room.Center;

			var x = (int) Math.Floor(center.X / 16);
			var y = (int) Math.Floor(center.Y / 16);
			var p = new Vector2(x * 16 + 8, y * 16 + 8);
			
			exit.Center = p;

			Painter.Fill(Run.Level, x - 1, y - 1, 3, 3, Tiles.RandomFloor());
			Painter.Fill(Run.Level, x - 1, y - 3, 3, 3, Tiles.RandomFloor());

			var stand = new BossStand();
			Area.Add(stand);
			stand.Center = p - new Vector2(0, 32f);
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Boss), Area), null);
			
			Run.Level.ReTileAndCreateBodyChunks(x - 1, y - 1, 3, 7);

			var rewards = new List<string>();

			for (var i = 0; i < Rnd.Int(2, 5); i++) {
				rewards.Add("bk:emerald");
			}
			
			for (var i = 0; i < Rnd.Int(0, 3); i++) {
				rewards.Add("bk:heart");
			}

			var j = 0;

			foreach (var reward in rewards) {
				var item = Items.CreateAndAdd(reward, Area);
				item.Center = stand.Center + MathUtils.CreateVector(j / ((float) rewards.Count) * Math.PI * 2 + Rnd.Float(-0.1f, 0.1f), Rnd.Float(12, 18));
				j++;
			}
		}

		public virtual void SelectAttack() {
			
		}

		public class DefeatedState : SmartState<Boss> {
			
		}

		public class FriendlyState : SmartState<Boss> {
		
		}

		public override bool IsFriendly() {
			return GetComponent<StateComponent>().StateInstance is FriendlyState;
		}

		public class DefeatedEvent : Event {
			public Boss Boss;
		}
	}
}