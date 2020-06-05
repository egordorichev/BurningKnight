using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.use;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
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
				Timer.Add(() => {
					var mob = MobRegistry.Generate();
					entity.Area.Add(mob);

					if (MobRegistry.FindFor(mob.GetType())?.NearWall ?? false) {
						mob.Center = r.Room.GetRandomFreeTileNearWall(filter) * 16;
					} else {
						mob.Center = r.Room.GetRandomFreeTile(filter) * 16;
					}

					var where = mob.Center;
					
					for (var j = 0; j < 8; j++) {
						var part = new ParticleEntity(Particles.Dust());
						
						part.Position = where + Rnd.Vector(-8, 8);
						part.Particle.Scale = Rnd.Float(1f, 1.3f);
						part.Particle.Velocity = MathUtils.CreateVector(Rnd.AnglePI(), 40);
						Run.Level.Area.Add(part);
						part.Depth = 1;
					}
					
					Audio.PlaySfx("scroll");
				}, (i - 1) * 0.2f);
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