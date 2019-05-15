using System;
using System.IO;
using BurningKnight.assets;
using BurningKnight.ui.imgui.node;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.ui.imgui {
	public static class DialogEditor {
		public static void Init() {
			ImGuiHelper.ClearNodes();
			Load();
		}

		public static void Destroy() {
			SaveDialogs();
			ImGuiHelper.ClearNodes();
		}
	
		private static void SaveDialogs() {
			var nodes = ImGuiHelper.Nodes;
			var root = new JsonArray();

			foreach (var p in nodes) {
				var node = p.Value;
				
				var obj = new JsonObject();
				node.Save(obj);

				root.Add(obj);
			}

			var file = File.CreateText("Content/Dialogs/dialogs.json");
			var writer = new JsonWriter(file, true);
			writer.Write(root);
			file.Close();
		}

		public static void Load() {
			try {
				LoadFromRoot(JsonValue.Parse(FileHandle.FromRoot("Dialogs/dialogs.json").ReadAll()));
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		private static void LoadFromRoot(JsonArray root) {
			foreach (var vl in root) {
				ImNode.Create(vl);
			}
		}

		public static void Render() {
			ImGuiHelper.RenderNodes();
		}
	}
}