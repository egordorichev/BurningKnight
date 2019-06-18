using System;
using BurningKnight.assets;
using BurningKnight.entity.projectile;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace BurningKnight.entity.item.use {
	public class SpawnProjectilesUse : ItemUse {
		private int damage;
		private float speed;
		private float range;
		private string slice;
		private int amount;
		
		public override void Use(Entity entity, Item item) {
			var s = range * 0.5f / speed;
			
			for (var i = 0; i < amount; i++) {
				var angle = (float) i / amount * Math.PI * 2;
				var projectile = Projectile.Make(entity, slice, angle, speed);

				projectile.AddLight(32f, Color.Yellow);
				projectile.Damage = damage;

				if (range > 0.01f) {
					projectile.Range = s;
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			damage = settings["damage"].Int(1);
			amount = settings["amount"].Int(1);
			speed = settings["speed"].Number(60);
			range = settings["range"].Number(0);
			slice = settings["texture"].AsString;
		}

		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
			
			val = root["damage"].Int(1);

			if (ImGui.InputInt("Damage", ref val)) {
				root["damage"] = val;
			}
			
			var spd = (float) root["speed"].Number(60);

			if (ImGui.InputFloat("Speed", ref spd)) {
				root["speed"] = spd;
			}
			
			var range = (float) root["range"].Number(0);

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