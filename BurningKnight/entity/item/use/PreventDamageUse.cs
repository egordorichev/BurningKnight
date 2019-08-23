using BurningKnight.entity.events;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class PreventDamageUse : ItemUse {
		private bool spikes;
		private bool lava;
		private bool chasm;

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if ((spikes && ev.From is Spikes)
				    || (lava && ev.From is Level)
				    || (chasm && ev.From is Chasm)) {
					
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			lava = settings["lv"].Bool(false);
			spikes = settings["sp"].Bool(true);
			chasm = settings["cs"].Bool(true);
		}

		public static void RenderDebug(JsonValue root) {
			var v = root["lv"].Bool(false);

			if (ImGui.Checkbox("From lava", ref v)) {
				root["lv"] = v;
			}
			
			v = root["sp"].Bool(false);

			if (ImGui.Checkbox("From spikes", ref v)) {
				root["sp"] = v;
			}
			
			v = root["cs"].Bool(false);

			if (ImGui.Checkbox("From chasm", ref v)) {
				root["cs"] = v;
			}
		}
	}
}