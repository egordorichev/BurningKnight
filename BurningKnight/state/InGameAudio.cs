using System.Linq;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.state {
	public class InGameAudio : Entity {
		public override void Init() {
			base.Init();

			AudioEmitterComponent.Listener = new AudioListener();
			
			Subscribe<GramophoneBrokenEvent>();
			Subscribe<Gramophone.DiskChangedEvent>();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<SecretRoomFoundEvent>();
			Subscribe<DiedEvent>();
			Subscribe<HealthModifiedEvent>();
			
			Subscribe<DoorClosedEvent>();
			Subscribe<DoorOpenedEvent>();
			Subscribe<LockOpenedEvent>();
			Subscribe<LockClosedEvent>();
			
			Subscribe<BombPlacedEvent>();
			Subscribe<ItemAddedEvent>();
			Subscribe<ChestOpenedEvent>();
			
			Subscribe<PlayerRolledEvent>();
			Subscribe<BurningKnightDefeatedEvent>();

			Subscribe<SpawnTrigger.TriggeredEvent>();
			
			/*var p = LocalPlayer.Locate(Area);
			var gramophone = p?.GetComponent<RoomComponent>().Room?.Tagged[Tags.Gramophone].FirstOrDefault();

			if (gramophone != null) {
				var t = ((Gramophone) gramophone).GetTune();
					
				if (t != null) {
					Audio.PlayMusic(t);
				} else {
					Audio.Stop();
				}

				return;
			}

			Audio.PlayMusic(Run.Level.GetMusic());*/
			
			// Audio.PlayMusic("Disk 6", Camera.Instance.Listener, LocalPlayer.Locate(Area).GetComponent<AudioEmitterComponent>().Emitter);
		}

		public override void Destroy() {
			base.Destroy();
			AudioEmitterComponent.Listener = null;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var c = Camera.Instance;
			AudioEmitterComponent.Listener.Position = new Vector3(c.PositionX, 0, c.PositionY);
		}

		public override bool HandleEvent(Event e) {
			if (true) {
				return false;
			}
			
			if (e is GramophoneBrokenEvent ge) {
				var local = LocalPlayer.Locate(ge.Gramophone.Area);

				if (local != null && ge.Gramophone.GetComponent<RoomComponent>().Room ==
				    local.GetComponent<RoomComponent>().Room) {
	
					Audio.Stop();
				}
			} else if (e is Gramophone.DiskChangedEvent gdce) {
				var t = gdce.Gramophone.GetTune();
					
				if (t != null) {
					Audio.PlayMusic(t);
				} else {
					Audio.Stop();
				}
			} else if (e is RoomChangedEvent re && re.Who is LocalPlayer) {
				var gramophone = re.New.Tagged[Tags.Gramophone].FirstOrDefault();

				if (gramophone != null) {
					var t = ((Gramophone) gramophone).GetTune();
					
					if (t != null) {
						Audio.PlayMusic(t);
					} else {
						Audio.Stop();
					}

					return false;
				}
				
				switch (re.New.Type) {
					case RoomType.Boss: {
						Audio.FadeOut();
						
						Timer.Add(() => {
							if (Area.Tags[Tags.BurningKnight].Count > 0 && ((Boss) Area.Tags[Tags.BurningKnight][0]).Awoken) {
								Audio.PlayMusic("Fatiga");
							} else {
								Audio.PlayMusic("Gobbeon");
							}
						}, 1f);
						break;
					}
					
					case RoomType.Treasure: {
						Audio.PlayMusic("Ma Precious");
						break;
					}
					
					case RoomType.Secret: {
						Audio.PlayMusic("Serendipity");
						break;
					}

					case RoomType.Shop: {
						Audio.PlayMusic("Shopkeeper");
						break;
					}

					default: {
						Audio.PlayMusic(Run.Level.GetMusic());
						break;
					}
				}
			} else if (e is SecretRoomFoundEvent) {
				Audio.Stop();
				
				Audio.PlaySfx("secret");
				Audio.PlaySfx("secret_room");
			} else if (e is DiedEvent de) {
				if (de.Who is Player) {
					Audio.Stop();
					Audio.PlayMusic("Nostalgia");
				} else if (!(de.Who is entity.creature.bk.BurningKnight)) {
					Audio.PlaySfx(de.Who, "enemy_death");
				}
			} else if (e is BurningKnightDefeatedEvent) {
				Audio.Stop();
				Audio.PlayMusic("Reckless");
				Audio.Repeat = false;
			} else if (e is HealthModifiedEvent he) {
				if (he.Amount < 0) {
					if (he.Who is entity.creature.bk.BurningKnight) {
						Audio.PlaySfx(he.Who, $"BK_hurt_{Random.Int(1, 6)}");
					} else if (he.Who is Creature) {
						Audio.PlaySfx(he.Who, he.Who is Player ? $"voice_gobbo_{Random.Int(1, 4)}" : "enemy_impact");
					}
				}
			} else if (e is DoorClosedEvent dce) {
				Audio.PlaySfx(dce.Who, "door_close");
			} else if (e is DoorOpenedEvent doe) {
				Audio.PlaySfx(doe.Who, "door_open");
			} else if (e is LockOpenedEvent loe) {
				Audio.PlaySfx(loe.Lock, loe.Lock is IronLock ? "door_unlock" : "door_lock");
			} else if (e is LockClosedEvent lce) {
				Audio.PlaySfx(lce.Lock, "door_lock");
			} else if (e is BombPlacedEvent bpe) {
				// FIXME: not working
				Audio.PlaySfx(bpe.Bomb, "bomb_placed");
			} else if (e is ItemAddedEvent iad) {
				if (iad.Who is Player) {
					switch (iad.Item.Type) {
						case ItemType.Coin: {
							Audio.PlaySfx("coin");
							break;
						}
						
						case ItemType.Heart: {
							Audio.PlaySfx("heart");
							break;
						}
						
						case ItemType.Bomb: {
							// fixme: add different sfx
							Audio.PlaySfx("heart");
							break;
						}
						
						case ItemType.Key: {
							Audio.PlaySfx("key");
							break;
						}

						default: {
							Audio.PlaySfx("pickup_item");
							break;
						}
					}
				}
			} else if (e is PlayerRolledEvent) {
				Audio.PlaySfx("gobbo_jump");
			} else if (e is ChestOpenedEvent coe) {
				Audio.PlaySfx(coe.Chest, "chest_open");
			} else if (e is SpawnTrigger.TriggeredEvent) {
				Audio.Stop();
			}

			return false;
		}
	}
}