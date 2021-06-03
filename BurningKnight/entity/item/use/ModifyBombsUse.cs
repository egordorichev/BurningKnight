using System;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class ModifyBombsUse : ItemUse {
		public bool SpawnBullets;
		public bool SpawnBombs;
		public bool SetFuseTime;
		public float FuseTime;
		public float RadiusMod;

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			SpawnBullets = settings["spawn_bullets"].Bool(false);
			SpawnBombs = settings["spawn_bombs"].Bool(false);
			SetFuseTime = settings["set_fuse"].Bool(false);
			FuseTime = settings["fuse_time"].Number(1);
			RadiusMod = settings["radius"].Number(1);
		}

		public static void RenderDebug(JsonValue root) {
			var spawnBullets = root["spawn_bullets"].Bool(false);

			if (ImGui.Checkbox("Spawn Bullets?", ref spawnBullets)) {
				root["spawn_bullets"] = spawnBullets;
			}
			
			var spawnBombs = root["spawn_bombs"].Bool(false);

			if (ImGui.Checkbox("Spawn Bombs?", ref spawnBombs)) {
				root["spawn_bombs"] = spawnBombs;
			}
			
			root.InputFloat("Radius Modifier", "radius", 1f);

			var setFuseTime = root["set_fuse"].Bool(false);

			if (ImGui.Checkbox("Set fuse?", ref setFuseTime)) {
				root["set_fuse"] = setFuseTime;
			}

			if (!setFuseTime) {
				return;
			}
			
			var fuseTime = root["fuse_time"].Number(1);

			if (ImGui.InputFloat("Fuse time", ref fuseTime)) {
				root["fuse_time"] = fuseTime;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is BombPlacedEvent bpe) {
				var bomb = bpe.Bomb;
				var c = bomb.GetComponent<ExplodeComponent>();

				c.Radius *= RadiusMod;
				
				if (SetFuseTime) {
					c.Timer = FuseTime + Rnd.Float(-0.1f, 1f);
				}

				if (SpawnBombs) {
					bomb.OnDeath += b => {
						if (b.Parent != null) {
							return;
						}

						for (var i = 0; i < 4; i++) {
							var bm = new Bomb(b.Owner, Bomb.ExplosionTime, b);
							b.Area.Add(bm);
							bm.Center = b.Center + Rnd.Vector(-4, 4);
							bm.VelocityTo(i / 2f * (float) Math.PI, 300f);
						}
					};
				}

				if (SpawnBullets) {
					bomb.OnDeath += b => {
						var builder = new ProjectileBuilder(b.Owner, "rect") {
							Scale = b.Scale
						};

						for (var i = 0; i < 8; i++) {
							builder.Shoot((float) i / 8 * (float) Math.PI * 2, 8).Build().Center = b.Center;
						}
					};
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}