using System;
using System.Collections.Generic;
using BurningKnight.ui.dialog;
using BurningKnight.ui.imgui;
using BurningKnight.ui.imgui.node;
using Lens.assets;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets {
	public static class Dialogs {
		private static Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

		public static void RegisterCallback(string id, Func<Dialog, DialogComponent, Dialog> callback) {
			Get(id)?.Callbacks.Add(callback);
		}
		
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
							}
						}
						
						// Connect em
						foreach (var node in ImNodes.Nodes) {
							node.Value.ReadOutputs();
						}
						
						// Parse
						foreach (var node in ImNodes.Nodes) {
							ParseNode(node.Value);
						}
						
						ImNodes.Nodes.Clear();
						ImNode.LastId = 0;
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
				return null;
			}

			return dialog;
		}
	}
}