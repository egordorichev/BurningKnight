using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using ImGuiNET;
using Lens.input;
using Lens.lightJson;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

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

				projectile.AddLight(32f, Color.Red);
				projectile.Damage = damage;
				projectile.Range = range * 0.5f / speed;
			};
		}
		
		
		public static void RenderDebug(JsonValue root) {
			var val = root["damage"].AsInteger;

			if (ImGui.InputInt("Damage", ref val)) {
				root["damage"] = val;
			}
			
			var spd = (float) root["speed"].AsNumber;

			if (ImGui.InputFloat("Speed", ref spd)) {
				root["speed"] = spd;
			}
			
			var range = (float) root["range"].AsNumber;

			if (ImGui.InputFloat("Range", ref range)) {
				root["range"] = range;
			}
			
			var slice = root["texture"].AsString;
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