using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
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

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			SpawnBullets = settings["spawn_bullets"].Bool(false);
			SpawnBombs = settings["spawn_bombs"].Bool(false);
			SetFuseTime = settings["set_fuse"].Bool(false);
			FuseTime = settings["fuse_time"].Number(1);
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
				
				if (SetFuseTime) {
					bomb.GetComponent<ExplodeComponent>().Timer = FuseTime + Rnd.Float(-0.1f, 1f);
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
						for (var i = 0; i < 8; i++) {
							Projectile.Make(b.Owner, "default", (float) i / 8 * (float) Math.PI * 2, 8, true, 0, null, b.Scale).Center = b.Center;
						}
					};
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}