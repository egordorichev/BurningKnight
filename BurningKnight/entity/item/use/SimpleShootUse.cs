using System;
using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using ImGuiNET;
using Lens.entity;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Num = System.Numerics;
using Random = Lens.util.math.Random;

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
		private bool wait;
		
		public bool ProjectileDied = true;

		public override void Use(Entity entity, Item item) {
			if (wait && !ProjectileDied) {
				return;
			}
			
			base.Use(entity, item);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
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
				var a = entity.AngleTo(Input.Mouse.GamePosition);
				var pr = prefab.Length == 0 ? null : ProjectileRegistry.Get(prefab);

				for (var i = 0; i < count; i++) {
					var angle = a;
					
					if (count == 1 || i > 0) {
						angle += Random.Float(-accuracy / 2f, accuracy / 2f);
					}

					var antiAngle = angle - (float) Math.PI;
					var projectile = Projectile.Make(entity, slice, angle, Random.Float(speed, speedMax), !rect, 0, null, Random.Float(scaleMin, scaleMax));

					Camera.Instance.Push(antiAngle, 4f);
					entity.GetAnyComponent<BodyComponent>()?.KnockbackFrom(antiAngle, 0.2f * knockback);

					if (light) {
						projectile.AddLight(32f, Color.Yellow);
					}

					projectile.Damage = damage;

					if (range > 0.01f) {
						projectile.Range = range * 0.5f / speed;
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
				entity.Area.Add(p);

				var f = (entity.CenterX > Input.Mouse.GamePosition.X ? 1 : -1);

				p.Particle.Velocity =
					new Vector2(f * Random.Float(40, 50), 0) + entity.GetAnyComponent<BodyComponent>().Velocity;

				p.Particle.Angle = 0;
				p.Particle.Zv = Random.Float(1.5f, 2.5f);
				p.Particle.AngleVelocity = f * Random.Float(40, 70);

				p.AddShadow();
			};
		}

		public static void RenderDebug(JsonValue root) {
			var val = root["damage"].Int(1);

			if (ImGui.InputInt("Damage", ref val)) {
				root["damage"] = val;
			}
			
			var amount = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref amount)) {
				root["amount"] = amount;
			}
			
			var spd = (float) root["speed"].Number(6);

			if (ImGui.InputFloat("Speed", ref spd)) {
				root["speed"] = spd;
			}
			
			var spdm = (float) root["speedm"].Number(10);

			if (ImGui.InputFloat("Max Speed", ref spdm)) {
				root["speedm"] = spdm;
			}
			
			var scale = (float) root["scale"].Number(1);

			if (ImGui.InputFloat("Min Scale", ref scale)) {
				root["scale"] = scale;
			}
			
			var scalem = (float) root["scalem"].Number(1);

			if (ImGui.InputFloat("Max Scale", ref scalem)) {
				root["scalem"] = scalem;
			}

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
		}
	}
}