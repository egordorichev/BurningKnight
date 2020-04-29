using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
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

		protected bool Died;
		private float deathTimer;
		private float lastExplosion;

		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new DialogComponent());

			var b = GetComponent<BuffsComponent>();
			b.AddImmunity<CharmedBuff>();
			b.AddImmunity<FrozenBuff>();
			
			Become<FriendlyState>();

			GetComponent<HealthComponent>().AutoKill = true;
			
			AddTag(Tags.Boss);
		}
		
		private bool cleared;

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Died) {
				if (!cleared) {
					cleared = true;
					GetComponent<DialogComponent>().Close();
					
					foreach (var p in Area.Tagged[Tags.Projectile]) {
						AnimationUtil.Poof(p.Center);
						((Projectile) p).Break(null);
					}

					try {
						var a = GetComponent<RoomComponent>().Room.Tagged[Tags.MustBeKilled].ToArray();

						foreach (var p in a) {
							if (!(p is Boss || p is bk.BurningKnight)) {
								AnimationUtil.Poof(p.Center);
								((Creature) p).Kill(this);
							}
						}
					} catch (Exception e) {
						Log.Error(e);
					}
				}

				if (deathTimer >= 3f) {
					CreateGore(null);
					HandleEvent(new DefeatedEvent {
						Boss = this
					});

					var player = LocalPlayer.Locate(Area);
					var doors = new List<DoorTile>();
					
					if (player != null) {
						var stats = player.GetComponent<StatsComponent>();
						var e = new DealChanceCalculateEvent();
						
						if (!stats.TookDamageInRoom) {
							Achievements.Unlock("bk:dodge_master");
						}
						
						e.GrannyStartChance = stats.SawDeal && !stats.TookDeal ? stats.GrannyChance : 0;
						e.GrannyChance = e.GrannyStartChance;
						e.DmStartChance = stats.DMChance;
						e.DmChance = e.DmStartChance;

						player.HandleEvent(e);

						var gr = Rnd.Chance(e.GrannyChance * 100);
						var dm = Rnd.Chance(e.DmChance * 100);
						
						if (gr || (dm && e.OpenBoth)) {
							foreach (var r in Area.Tagged[Tags.Room]) {
								var room = (Room) r;

								if (room.Type == RoomType.Granny) {
									room.OpenHiddenDoors();

									foreach (var d in room.Doors) {
										doors.Add(new DoorTile {
											Door = d,
											Tile = Tile.GrannyFloor
										});										
									}
									
									break;
								}
							}
						}
						
						if (dm || (gr && e.OpenBoth)) {
							foreach (var r in Area.Tagged[Tags.Room]) {
								var room = (Room) r;

								if (room.Type == RoomType.OldMan) {
									room.OpenHiddenDoors();

									foreach (var d in room.Doors) {
										doors.Add(new DoorTile {
											Door = d,
											Tile = Tile.EvilFloor
										});										
									}
									
									break;
								}
							}
						}
					}

					if (doors.Count > 0) {
						var rm = GetComponent<RoomComponent>().Room;
						var level = Run.Level;
						var cx = rm.MapX + rm.MapW / 2f;
						var cy = rm.MapY + rm.MapH / 2f;
						var grannyDoors = new List<Door>();
						var evilDoors = new List<Door>();

						foreach (var d in doors) {
							if (d.Tile == Tile.GrannyFloor) {
								grannyDoors.Add(d.Door);
							} else {
								evilDoors.Add(d.Door);
							}
						}
						
						rm.PaintTunnel(grannyDoors, Tile.GrannyFloor);
						rm.PaintTunnel(evilDoors, Tile.EvilFloor);

						rm.ApplyToEachTile((x, y) => {
							var t = level.Get(x, y);
							
							if (t == Tile.GrannyFloor || t == Tile.EvilFloor) {
								level.Set(x, y, Tile.FloorA);
								
								Timer.Add(() => {
									var part = new TileParticle();

									part.Top = t == Tile.GrannyFloor ? Tilesets.Biome.GrannyFloor[0] : Tilesets.Biome.EvilFloor[0];
									part.TopTarget = Run.Level.Tileset.WallTopADecor;
									part.Side = Run.Level.Tileset.FloorSidesD[0];
									part.Sides = Run.Level.Tileset.WallSidesA[2];
									part.Tile = t;

									part.X = x * 16;
									part.Y = y * 16;
									part.Target.X = x * 16;
									part.Target.Y = y * 16;
									part.TargetZ = -8f;

									Area.Add(part);
								}, 1f + Rnd.Float(0.2f) + MathUtils.Distance(x - cx, y - cy) / 6f);
							}
						});
						
						level.TileUp();
						level.CreateBody();
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
						Audio.PlaySfx($"level_explosion_{Rnd.Int(1, 4)}");
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
				AddPhases();
			}
		}

		private struct DoorTile {
			public Door Door;
			public Tile Tile;
		}

		protected virtual void AddPhases() {
			
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
			if (e is DiedEvent de) {
				if (de.Who == this) {
					if (!Died) {
						Died = true;
						HealthBar?.Remove();

						Camera.Instance.Targets.Clear();
						Camera.Instance.Follow(this, 1f);
						Become<DefeatedState>();

						Audio.Stop();
					}

					Done = false;
					e.Handled = true;
				}
			}

			return base.HandleEvent(e);
		}

		public virtual void PlaceRewards() {
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

			Run.Level.ReTileAndCreateBodyChunks(x - 1, y - 1, 3, 7);
			var w = p - new Vector2(0, 32f);

			if (Run.Type != RunType.BossRush) {
				var stand = new BossStand();
				Area.Add(stand);
				stand.Center = w;
				stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Boss), Area), null);
			}

			var rewards = new List<string>();
			var c = Run.Type == RunType.BossRush ? 2 : Rnd.Int(2, 5);

			if (Run.Type != RunType.BossRush) {
				for (var i = 0; i < c; i++) {
					rewards.Add("bk:emerald");
				}

				var q = Rnd.Int(4, 10);
				
				for (var i = 0; i < q; i++) {
					rewards.Add("bk:copper_coin");
				}
			}

			var cn = Run.Type == RunType.BossRush ? Rnd.Int(2, 5) : Rnd.Int(0, 3);
			
			for (var i = 0; i < cn; i++) {
				rewards.Add("bk:heart");
			}

			var j = 0;

			foreach (var reward in rewards) {
				var item = Items.CreateAndAdd(reward, Area);
				item.Center = w + MathUtils.CreateVector(j / ((float) rewards.Count) * Math.PI * 2 + Rnd.Float(-0.1f, 0.1f), Rnd.Float(12, 18));
				j++;
			}
		}

		public virtual void SelectAttack() {
			
		}

		public class DefeatedState : SmartState<Boss> {
			
		}

		public class FriendlyState : SmartState<Boss> {
			public override void Init() {
				base.Init();
				Self.GetComponent<HealthComponent>().Unhittable = true;
			}

			public override void Destroy() {
				base.Destroy();

				if (!(this is bk.BurningKnight)) {
					Self.GetComponent<HealthComponent>().Unhittable = false;
				}
			}
		}

		public override bool IsFriendly() {
			return GetComponent<StateComponent>().StateInstance is FriendlyState;
		}

		public class DefeatedEvent : Event {
			public Boss Boss;
		}
		
		public override void Kill(Entity w) {
			
		}

		public virtual string GetScream() {
			return "bk_3";
		}
	}
}