using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.use;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class SpawnMobsUse : ItemUse {
		public int Count;

		private static Func<int, int, bool> CheckDistance(Entity entity) {
			return (x, y) => entity.DistanceTo(new Vector2(x * 16, y * 16)) > 32;
		}
		
		public override void Use(Entity entity, Item item) {
			if (!entity.TryGetComponent<RoomComponent>(out var r) || r.Room == null) {
				return;
			}

			var filter = CheckDistance(entity);
			MobRegistry.SetupForBiome(Run.Level.Biome.Id);
			
			for (var i = 0; i < Count; i++) {
				var mob = MobRegistry.Generate();
				entity.Area.Add(mob);

				if (MobRegistry.FindFor(mob.GetType())?.NearWall ?? false) {
					mob.Center = r.Room.GetRandomFreeTileNearWall(filter) * 16;
				} else {
					mob.Center = r.Room.GetRandomFreeTile(filter) * 16;
				}

				AnimationUtil.Poof(mob.Center, 1);
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Count = settings["count"].Int(1);
		}

		public static void RenderDebug(JsonValue root) {
			var v = root["count"].Int(1);

			if (ImGui.InputInt("Count", ref v)) {
				root["count"] = v;
			}
		}
	}
}