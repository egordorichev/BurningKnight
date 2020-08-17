using System.Linq;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.biome;
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

			AlwaysActive = true;

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
			
			Subscribe<PlayerRolledEvent>();
			Subscribe<Boss.DefeatedEvent>();

			Subscribe<SpawnTrigger.TriggeredEvent>();
			
			var p = LocalPlayer.Locate(Area);
			var gramophone = p?.GetComponent<RoomComponent>().Room?.Tagged[Tags.Gramophone].FirstOrDefault();

			if (gramophone != null) {
				var t = ((Gramophone) gramophone).GetTune();
					
				if (t != null) {
					Audio.PlayMusic(t);
				} else {
					Audio.FadeOut();
				}

				return;
			}

			if (!InGameState.InMenu && Run.Level != null) {
				Audio.PlayMusic(Run.Level.GetMusic(), Run.Depth < 1 || Run.Depth % 2 == 1);
			}
			
			// Audio.PlayMusic("Disk 6", Camera.Instance.Listener, LocalPlayer.Locate(Area).GetComponent<AudioEmitterComponent>().Emitter);
		}

		public override void Destroy() {
			base.Destroy();
			AudioEmitterComponent.Listener = null;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var c = Camera.Instance;
			
			AudioEmitterComponent.ListenerPosition = new Vector2(c.PositionX, c.PositionY);
			AudioEmitterComponent.Listener.Position = new Vector3(c.PositionX * AudioEmitterComponent.PositionScale, 0, c.PositionY * AudioEmitterComponent.PositionScale);
		}

		public override bool HandleEvent(Event e) {
			if (InGameState.InMenu) {
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
					Audio.FadeOut();
				}
			} else if (e is RoomChangedEvent re && re.Who is LocalPlayer) {
				var gramophone = re.New.Tagged[Tags.Gramophone].FirstOrDefault();

				if (gramophone != null) {
					var t = ((Gramophone) gramophone).GetTune();
					
					if (t != null) {
						Audio.PlayMusic(t);
					} else {
						Audio.FadeOut();
					}

					return false;
				}
				
				switch (re.New.Type) {
					case RoomType.Boss: {
						if (Run.Level.Biome is TechBiome) {
							Audio.PlayMusic(Run.Level.Biome.GetMusic());
						} else {
							if (Area.Tagged[Tags.Boss].Count > 0 && ((Boss) Area.Tagged[Tags.Boss][0]).Awoken) {
								Audio.PlayMusic((Run.Level.Biome is LibraryBiome || Run.Level.Biome is LibraryBiome)
									? "Last chance"
									: "Fatiga");
							} else {
								Audio.PlayMusic("Gobbeon");
							}
						}

						break;
					}
					
					case RoomType.DarkMarket: {
						if (Run.AlternateMusic) {
							Audio.PlayMusic("piano");
							break;
						}

						Audio.PlayMusic("Serendipity");
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
					
					case RoomType.OldMan: {
						Audio.PlayMusic("Gobbeon");
						break;
					}
					
					case RoomType.Granny: {
						Audio.PlayMusic("Disk 1");
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
			} else if (e is DiedEvent de) {
				if (de.Who is Player pl && InGameState.EveryoneDied(pl)) {
					Audio.Stop();
					Audio.PlayMusic("Nostalgia");
				}
			} else if (e is Boss.DefeatedEvent) {
				Audio.Stop();
				Audio.Repeat = false;
				Audio.PlayMusic("Reckless");
			}

			return false;
		}
	}
}