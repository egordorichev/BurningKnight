using System;
using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using ImGuiNET;
using Lens.assets;
using Lens.input;
using Lens.lightJson;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Num = System.Numerics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			var damage = settings["damage"].Int(1);
			var speed = settings["speed"].Number(60);
			var range = settings["range"].Number(96);
			var slice = settings["texture"].AsString;
			
			SpawnProjectile = (entity, item) => {
				var angle = entity.AngleTo(Input.Mouse.GamePosition);
				var antiAngle = angle - (float) Math.PI;
				var projectile = Projectile.Make(entity, slice, angle, speed);

				Camera.Instance.Push(antiAngle, 4f);
				entity.GetAnyComponent<BodyComponent>()?.KnockbackFrom(antiAngle, 0.2f);

				projectile.AddLight(32f, Color.Yellow);
				projectile.Damage = damage;
				projectile.Range = range * 0.5f / speed;
				
				var p = new ParticleEntity(new Particle(Controllers.Destroy, new TexturedParticleRenderer {
					Region = CommonAse.Particles.GetSlice("shell")
				}));

				p.Position = entity.Center;
				entity.Area.Add(p);

				var f = (entity.CenterX > Input.Mouse.GamePosition.X ? 1 : -1);
				
				p.Particle.Velocity = new Vector2(f * Random.Float(40, 50), 0) + entity.GetAnyComponent<BodyComponent>().Velocity;
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
			
			var spd = (float) root["speed"].Number(60);

			if (ImGui.InputFloat("Speed", ref spd)) {
				root["speed"] = spd;
			}
			
			var range = (float) root["range"].Number(96);

			if (ImGui.InputFloat("Range", ref range)) {
				root["range"] = range;
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