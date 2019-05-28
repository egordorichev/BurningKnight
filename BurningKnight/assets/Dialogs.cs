using System;
using System.Collections.Generic;
using BurningKnight.ui.dialog;
using BurningKnight.ui.imgui.node;
using Lens.assets;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets {
	public static class Dialogs {
		private static Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

		public static void Load() {
			var dir = FileHandle.FromRoot("Dialogs");
			
			foreach (var f in dir.ListFileHandles()) {
				if (f.Extension == ".json") {
					try {
						var name = f.NameWithoutExtension;
						var root = JsonValue.Parse(f.ReadAll());
						
						// Create nodes
						foreach (var vl in root.AsJsonArray) {
							try {
								ImNode.Create(name, vl);
							} catch (Exception e) {
								Log.Error(e);
							}
						}
						
						// Connect em
						foreach (var node in ImGuiHelper.Nodes) {
							node.Value.ReadOutputs();
						}
						
						// Parse
						foreach (var node in ImGuiHelper.Nodes) {
							ParseNode(node.Value);
						}
						
						ImGuiHelper.ClearNodes();
					} catch (Exception e) {
						Log.Error(e);
					}
				}
			}
		}

		private static void ParseNode(ImNode node) {
			if (node is DialogNode n) {
				Add(n.Convert());				
			}
		}

		public static void Add(Dialog dialog) {
			dialogs[dialog.Id] = dialog;
		}

		public static Dialog Get(string id) {
			if (!dialogs.TryGetValue(id, out var dialog)) {
				if (Locale.Contains(id)) {
					dialog = new Dialog(id);
					dialogs[id] = dialog;
				} else {
					Log.Error($"Unknown dialog {id}");
					return null;
				}
			}

			return dialog;
		}
	}
}