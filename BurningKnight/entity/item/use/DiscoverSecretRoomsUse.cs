using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.use {
	public class DiscoverSecretRoomsUse : ItemUse {
		private float chance;

		private void CheckRoom(Entity who, Room room) {
			var level = Run.Level;

			if (room != null) {
				for (var y = room.MapY; y < room.MapY + room.MapY; y++) {
					for (var x = room.MapX; x < room.MapX + room.MapW; x++) {
						if (level.IsInside(x, y) && level.Get(x, y) == Tile.Crack) {
							ExplosionMaker.DiscoverCrack(who, level, x, y);
						}
					}	
				}
			}
		}

		public override void Use(Entity entity, Item item) {
			CheckRoom(entity, entity.GetComponent<RoomComponent>().Room);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (!Random.Chance(chance)) {
					return base.HandleEvent(e);
				}

				CheckRoom(rce.Who, rce.New);
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			chance = settings["chance"].Number(100);
		}

		public static void RenderDebug(JsonValue root) {
			var chance = root["chance"].Number(100);

			if (ImGui.InputFloat("Chance", ref chance)) {
				root["chance"] = chance;
			}
		}
	}
}