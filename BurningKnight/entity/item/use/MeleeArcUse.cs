using BurningKnight.entity.item.util;
using ImGuiNET;
using Lens.entity;
using Lens.input;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MeleeArcUse : ItemUse {
		protected int Damage;
		protected float LifeTime;
		
		public override void Use(Entity entity, Item item) {
			entity.Area.Add(new MeleeArc {
				Owner = entity,
				LifeTime = LifeTime,
				Damage = Damage,
				Position = entity.Center,
				Angle = entity.AngleTo(Input.Mouse.GamePosition)
			});
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Damage = settings["damage"].Int(1);
			LifeTime = settings["time"].Number(0.2f);
		}

		public static void RenderDebug(JsonValue root) {
			var damage = root["damage"].AsInteger;
			
			if (ImGui.InputInt("Damage", ref damage)) {
				root["damage"] = damage;
			}
			
			var time = root["time"].AsNumber;
			
			if (ImGui.InputDouble("Life time", ref time)) {
				root["time"] = time;
			}
		}
	}
}