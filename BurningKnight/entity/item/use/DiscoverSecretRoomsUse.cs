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
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class DiscoverSecretRoomsUse : ItemUse {
		private float chance;

		public override void Use(Entity entity, Item item) {				
			ExplosionMaker.CheckForCracks(Run.Level, entity.GetComponent<RoomComponent>().Room, entity);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (!Rnd.Chance(chance)) {
					return base.HandleEvent(e);
				}

				ExplosionMaker.CheckForCracks(Run.Level, rce.New, rce.Who);
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