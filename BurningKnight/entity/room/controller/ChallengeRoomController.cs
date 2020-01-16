using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.room.controller {
	public class ChallengeRoomController : RoomController, Subscriber {
		private bool spawned;

		public override void Init() {
			base.Init();

			var e = Room.Area.EventListener;
			
			e.Subscribe<Chest.OpenedEvent>(this);
			e.Subscribe<ItemTakenEvent>(this);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(spawned);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			spawned = stream.ReadBoolean();
		}
		
		private static Func<int, int, bool> CheckDistance(Entity entity) {
			return (x, y) => entity.DistanceTo(new Vector2(x * 16, y * 16)) > 32;
		}

		private void Spawn(Entity entity) {
			spawned = true;
			
			var filter = CheckDistance(entity);
			MobRegistry.SetupForBiome(Run.Level.Biome.Id);

			var c = 6;
			
			for (var i = 0; i < c; i++) {
				var mob = MobRegistry.Generate();
				entity.Area.Add(mob);

				if (MobRegistry.FindFor(mob.GetType())?.NearWall ?? false) {
					mob.Center = Room.GetRandomFreeTileNearWall(filter) * 16;
				} else {
					mob.Center = Room.GetRandomFreeTile(filter) * 16;
				}
			}
		}

		public bool HandleEvent(Event e) {
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