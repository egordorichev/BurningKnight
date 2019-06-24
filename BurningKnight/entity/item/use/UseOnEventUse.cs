using System;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class UseOnEventUse : ItemUse {
		private string type;
		private Type typeInstance;
		private Entity owner;
		private Item item;
		private string use;
		private JsonValue options;

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			type = settings["tp"].String("");
			use = settings["use"].String("");
			options = settings["us"];

			if (options == JsonValue.Null) {
				options = new JsonObject();
			}

			try {
				typeInstance = Type.GetType(type, true, false);
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public override void Use(Entity entity, Item i) {
			owner = entity;
			item = i;
		}

		public override bool HandleEvent(Event e) {
			if (e.GetType() == typeInstance) {
				var u = UseRegistry.Create(use);

				if (u == null) {
					Log.Error($"{use} is invalid item use id");
				} else {
					u.Setup(options);
					u.Use(owner, item);
				}
			}
			
			return base.HandleEvent(e);
		}

		public static void RenderDebug(JsonValue root) {
			var type = root["tp"].String("");
			var use = root["use"].String("");
			
			if (ImGui.InputText("Use", ref use, 128)) {
				root["use"] = use;
			}

			var c = !UseRegistry.Uses.ContainsKey(use);

			if (c) {
				ImGui.BulletText("Unknown use");
			}
			
			if (ImGui.InputText("Event type", ref type, 256)) {
				root["tp"] = type;
			}

			try {
				Type.GetType(type, true, false);
			} catch (Exception e) {
				ImGui.BulletText("Unknown type");
			}
			
			if (c) {
				return;
			}
			
			var us = root["us"];

			if (us == JsonValue.Null) {
				us = root["us"] = new JsonObject();
			}

			us["id"] = use;
			
			ItemEditor.DisplayUse(root, us);
		}
	}
}