using BurningKnight.entity.item.util;
using ImGuiNET;
using Lens.entity;
using Lens.input;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MeleeArcUse : ItemUse {
		protected int Damage;
		protected float LifeTime;
		protected int W;
		protected int H;
		
		public override void Use(Entity entity, Item item) {
			entity.Area.Add(new MeleeArc {
				Owner = entity,
				LifeTime = LifeTime,
				Damage = Damage,
				Width = W,
				Height = H,
				Position = entity.Center,
				Angle = entity.AngleTo(Input.Mouse.GamePosition)
			});
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Damage = settings["damage"].Int(1);
			W = settings["w"].Int(8);
			H = settings["h"].Int(24);
			LifeTime = settings["time"].Number(0.2f);
		}

		public static void RenderDebug(JsonValue root) {
			var damage = root["damage"].Int(1);
			
			if (ImGui.InputInt("Damage", ref damage)) {
				root["damage"] = damage;
			}
			
			var w = root["w"].Int(8);
			
			if (ImGui.InputInt("Width", ref w)) {
				root["w"] = w;
			}	
			
			var h = root["h"].Int(24);
			
			if (ImGui.InputInt("Height", ref h)) {
				root["h"] = h;
			}
			
			var time = (double) root["time"].Number(0.2f);
			
			if (ImGui.InputDouble("Life time", ref time)) {
				root["time"] = time;
			}
		}
	}
}