using BurningKnight.entity.events;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class PreventDamageUse : ItemUse {
		private bool spikes;
		private bool lava;
		private bool chasm;
		private bool bombs;
		private bool contact;

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if ((spikes && ev.From is Spikes)
				    || (lava && ev.From is Level)
				    || (chasm && ev.From is Chasm)
				    || (bombs && ev.Type == DamageType.Explosive)
				    || (contact && ev.Type == DamageType.Contact)) {
					
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			lava = settings["lv"].Bool(false);
			spikes = settings["sp"].Bool(false);
			chasm = settings["cs"].Bool(false);
			bombs = settings["bms"].Bool(false);
			contact = settings["cnt"].Bool(false);
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
			
			v = root["bms"].Bool(false);

			if (ImGui.Checkbox("From explosions", ref v)) {
				root["bms"] = v;
			}

			root.Checkbox("From Contact", "cnt", false);
		}
	}
}