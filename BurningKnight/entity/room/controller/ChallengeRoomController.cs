using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.room.controller {
	public class ChallengeRoomController : RoomController, Subscriber {
		private bool spawned;
		private byte wave;

		public override void Init() {
			base.Init();

			var e = Room.Area.EventListener;
			
			e.Subscribe<Chest.OpenedEvent>(this);
			e.Subscribe<ItemTakenEvent>(this);
			e.Subscribe<DiedEvent>(this);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteBoolean(spawned);
			stream.WriteByte(wave);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			spawned = stream.ReadBoolean();
			wave = stream.ReadByte();
		}
		
		private static Func<int, int, bool> CheckDistance(Entity entity) {
			return (x, y) => entity == null || entity.DistanceTo(new Vector2(x * 16, y * 16)) > 32;
		}

		private void Spawn(Entity entity) {
			spawned = true;
			SpawnWave(entity);
		}

		private void SpawnWave(Entity entity) {
			var filter = CheckDistance(entity);
			MobRegistry.SetupForBiome(Run.Level.Biome.Id);

			var c = Rnd.Int(5, 11);
			
			for (var i = 0; i < c; i++) {
				Timer.Add(() => {
					var mob = MobRegistry.Generate();
					entity.Area.Add(mob);
					var v = MobRegistry.FindFor(mob.GetType());

					if (v?.NearWall ?? false) {
						mob.Center = Room.GetRandomFreeTileNearWall(filter) * 16;
					} else if (v?.AwayFromWall ?? false) {
						mob.Center = Room.GetRandomWallFreeTile(filter) * 16;
					} else {
						mob.Center = Room.GetRandomFreeTile(filter) * 16;
					}

					if (mob.TryGetComponent<ZAnimationComponent>(out var z)) {
						z.Animate();
					} else if (mob.TryGetComponent<MobAnimationComponent>(out var m)) {
						m.Animate();
					}
					
					AnimationUtil.Poof(mob.Center);
				}, (i) * 0.2f);
			}

			wave++;
		}

		public bool HandleEvent(Event e) {
			if (wave == 4) {
				return false;
			}
		
			if (e is DiedEvent de && de.Who.GetComponent<RoomComponent>().Room == Room) {
				foreach (var m in Room.Tagged[Tags.MustBeKilled]) {
					var mob = (Mob) m;

					if (!mob.GetComponent<HealthComponent>().HasNoHealth) {
						return false;
					}
				}
				
				SpawnWave(de.From);
				de.BlockClear = true;
			}
			
			if (spawned) {
				return false;
			}
			
			if (e is ItemTakenEvent ite) {
				if (ite.Stand.GetComponent<RoomComponent>().Room == Room) {
					Spawn(ite.Who);
				}
			} else if (e is Chest.OpenedEvent coe) {
				if (coe.Chest.GetComponent<RoomComponent>().Room == Room) {
					Spawn(coe.Who);
				}
			}
			
			return false;
		}
	}
}