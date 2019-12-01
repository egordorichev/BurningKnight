using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		private int damage;
		private float speed;
		private float speedMax;
		private float range;
		private float scaleMin;
		private float scaleMax;
		private string slice;
		private float accuracy;
		private int count;
		private string prefab;
		private bool light;
		private float knockback;
		private bool rect;
		protected bool wait;
		
		public bool ProjectileDied = true;

		private ItemUse[] modifiers;

		public override void Use(Entity entity, Item item) {
			if (wait && !ProjectileDied) {
				return;
			}
			
			base.Use(entity, item);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			modifiers = Items.ParseUses(settings["modifiers"]);

			if (modifiers != null) {
				foreach (var m in modifiers) {
					m.Item = Item;
				}
			}
			
			damage = settings["damage"].Int(1);
			speed = settings["speed"].Number(6);
			speedMax = settings["speedm"].Number(10);
			range = settings["range"].Number(0);
			scaleMin = settings["scale"].Number(1);
			scaleMax = settings["scalem"].Number(1);
			slice = settings["texture"].AsString;
			accuracy = settings["accuracy"].Number(0).ToRadians();
			count = settings["amount"].Int(1);
			prefab = settings["prefab"].String("");
			light = settings["light"].Bool(true);
			knockback = settings["knockback"].Number(1);
			rect = settings["rect"].Bool(false);
			wait = settings["wait"].Bool(false);

			SpawnProjectile = (entity, item) => {
				var bad = entity is Creature c && !c.IsFriendly();
				var sl = slice;
				
				if (bad) {
					sl = "small";
				}

				entity.TryGetComponent<StatsComponent>(out var stats);
				entity.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_gun_fire", 2, 0.5f);

				var aim = entity.GetComponent<AimComponent>();
				var from = aim.Center;
				var a = MathUtils.Angle(aim.RealAim.X - from.X, aim.RealAim.Y - from.Y);
				var pr = prefab.Length == 0 ? null : ProjectileRegistry.Get(prefab);
				var ac = accuracy;

				if (stats != null) {
					ac /= stats.Accuracy;
				}
				
				for (var i = 0; i < count; i++) {
					var angle = a;
					
					if (count == 1 || i > 0) {
						angle += Rnd.Float(-ac / 2f, ac / 2f);
					}

					var antiAngle = angle - (float) Math.PI;
					var projectile = Projectile.Make(entity, sl, angle, Rnd.Float(speed, speedMax), !rect, 0, null, Rnd.Float(scaleMin, scaleMax), damage, Item);

					Camera.Instance.Push(antiAngle, 4f);
					entity.GetComponent<RectBodyComponent>()?.KnockbackFrom(antiAngle, 0.4f * knockback);

					if (light) {
						projectile.AddLight(32f, bad ? Projectile.RedLight : Projectile.YellowLight);
					}

					projectile.FlashTimer = 0.05f;

					if (range > 0.01f) {
						if (Math.Abs(projectile.Range - (-1)) < 0.1f) {
							projectile.Range = range / speed;
						} else {
							projectile.Range += range / speed;
						}
					}
					
					projectile.Center = from;

					if (modifiers != null) {
						foreach (var m in modifiers) {
							if (m is ModifyProjectilesUse mpu) {
								mpu.ModifyProjectile(projectile);
							}
						}
					}

					pr?.Invoke(projectile);

					if (wait && i == 0) {
						ProjectileDied = false;
						projectile.OnDeath += (prj, t) => ProjectileDied = true;
					}
				}

				var p = new ParticleEntity(new Particle(Controllers.Destroy, new TexturedParticleRenderer {
					Region = CommonAse.Particles.GetSlice("shell")
				}));

				p.Position = entity.Center;
				p.Y += Rnd.Float(-4, 10);

				entity.Area.Add(p);

				var f = (entity.CenterX > Input.Mouse.GamePosition.X ? 1 : -1);

				p.Particle.Velocity =
					new Vector2(f * Rnd.Float(40, 60), 0) + entity.GetAnyComponent<BodyComponent>().Velocity;

				p.Particle.Angle = 0;
				p.Particle.Zv = Rnd.Float(1.5f, 2.5f);
				p.Particle.AngleVelocity = f * Rnd.Float(40, 70);

				p.AddShadow();
			};
		}

		public static void RenderDebug(JsonValue root) {
			if (ImGui.TreeNode("Stats")) {
				root.InputFloat("Damage", "damage");
				root.InputInt("Projectile Count", "amount");

				ImGui.Separator();

				root.InputFloat("Min Speed", "speed", 10);
				root.InputFloat("Max Speed", "speedm", 10);

				ImGui.Separator();
				
				root.InputFloat("Min Scale", "scale");
				root.InputFloat("Max Scale", "scalem");
				
				ImGui.Separator();

				var range = (float) root["range"].Number(0);

				if (ImGui.InputFloat("Range", ref range)) {
					root["range"] = range;
				}

				var knockback = (float) root["knockback"].Number(1);

				if (ImGui.InputFloat("Knockback", ref knockback)) {
					root["knockback"] = knockback;
				}

				var accuracy = (float) root["accuracy"].Number(0);

				if (ImGui.InputFloat("Accuracy", ref accuracy)) {
					root["accuracy"] = accuracy;
				}

				var light = root["light"].Bool(true);

				if (ImGui.Checkbox("Light", ref light)) {
					root["light"] = light;
				}

				var rect = root["rect"].Bool(false);

				if (ImGui.Checkbox("Rect body", ref rect)) {
					root["rect"] = rect;
				}

				var wait = root["wait"].Bool(false);

				if (ImGui.Checkbox("Wait for projectile death", ref wait)) {
					root["wait"] = wait;
				}

				var prefab = root["prefab"].String("");

				if (ImGui.InputText("Prefab", ref prefab, 128)) {
					root["prefab"] = prefab;
				}

				if (prefab.Length > 0 && ProjectileRegistry.Get(prefab) == null) {
					ImGui.BulletText("Unknown prefab");
				}

				var slice = root["texture"].String("");
				var region = CommonAse.Projectiles.GetSlice(slice);

				ImGui.Image(ImGuiHelper.ProjectilesTexture, new Num.Vector2(region.Width * 3, region.Height * 3),
					new Num.Vector2(region.X / region.Texture.Width, region.Y / region.Texture.Height),
					new Num.Vector2((region.X + region.Width) / region.Texture.Width,
						(region.Y + region.Height) / region.Texture.Height));

				if (ImGui.InputText("Texture", ref slice, 128)) {
					root["texture"] = slice;
				}

				ImGui.TreePop();
			}

			if (ImGui.TreeNode("Modifiers")) {
				if (!root["modifiers"].IsJsonArray) {
					root["modifiers"] = new JsonArray();
				}

				ItemEditor.DisplayUse(root, root["modifiers"], "bk:ModifyProjectiles");
				ImGui.TreePop();
			}
		}
	}
}