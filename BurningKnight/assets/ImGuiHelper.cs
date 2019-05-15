using System;
using System.Collections.Generic;
using BurningKnight.ui.imgui;
using BurningKnight.ui.imgui.node;
using ImGuiNET;
using Lens;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.assets {
	public static class ImGuiHelper {
		public static ImGuiRenderer Renderer;
		public static Dictionary<int, ImNode> Nodes = new Dictionary<int, ImNode>();
		
		public static void Init() {
			Renderer = new ImGuiRenderer(Engine.Instance);
		}

		private static List<int> toRemove = new List<int>();
		private static bool loadedFont;

		public static void Begin() {
			if (!loadedFont) {
				loadedFont = true;
				Renderer.RebuildFontAtlas();
			}
			
			Renderer.BeforeLayout(Engine.GameTime);
		}

		public static void End() {
			Renderer.AfterLayout();
		}

		public static void ClearNodes() {
			Nodes.Clear();
		}

		public static void Node(ImNode node) {
			Nodes[node.Id] = node;
		}

		public static void RenderNodes() {
			RenderVoidMenu();
			
			foreach (var n in Nodes) {
				var node = n.Value;
				
				node.Render();

				if (node.Done) {
					toRemove.Add(n.Key);
				}
			}

			if (toRemove.Count > 0) {
				foreach (var k in toRemove) {
					Nodes.Remove(k);
				}
				
				toRemove.Clear();
			}
			
			if (!ImGui.Begin("Nodes")) {
				ImGui.End();
				return;
			}

			RenderMenu();
			
			foreach (var p in Nodes) {
				var node = p.Value;

				if (ImNode.Focused == node) {
					node.ForceFocus = true;
				}
				
				if (ImGui.Selectable($"{node.GetName()} #{node.Id}", ImNode.Focused == node)) {
					ImNode.Focused = node;
					node.ForceFocus = true;
				}

				if (ImGui.OpenPopupOnItemClick("node_menu", 1)) {
					CurrentMenu = node;
				}
			}
			
			ImGui.End();

			if (pasted != null) {
				try {
					var root = JsonValue.Parse(pasted);

					if (root.IsJsonObject) {
						var val = root["imnode"];
						var node = ImNode.Create(val, true);

						if (node != null) {
							node.New = true;
							node.Position = ImGui.GetIO().MousePos;
							ImNode.Focused = node;
						}
					}
				} catch (Exception e) {
					Log.Error(e);
				}

				pasted = null;
			}
		}

		public static ImNode CurrentMenu;
		private static string pasted;

		public static void RenderPaste() {
			if (ImGui.Selectable("Paste (Ctrl+V)")) {
				Paste();
			}
		}
		
		public static void RenderVoidMenu() {
			if (ImGui.BeginPopupContextVoid("void_menu")) {
				RenderPaste();
			}

			if (Input.Keyboard.IsDown(Keys.LeftControl, true) || Input.Keyboard.IsDown(Keys.RightControl, true)) {
				if (Input.Keyboard.WasPressed(Keys.C, true)) {
					Copy(false);
				}
				
				if (Input.Keyboard.WasPressed(Keys.V, true)) {
					Paste();
				}
				
				if (Input.Keyboard.WasPressed(Keys.Delete, true) && ImNode.Focused != null) {
					ImNode.Focused.Remove();
				}	
			}
		}

		private static void Copy(bool menu = true) {
			var root = new JsonObject();

			if (menu && CurrentMenu != null) {
				CurrentMenu.Save(root);
			} else if (ImNode.Focused != null) {
				ImNode.Focused.Save(root);
			}
			
			ImGui.SetClipboardText($"{{ \"imnode\" : {root} }}");
		}

		private static void Paste() {
			try {
				var text = ImGui.GetClipboardText();
				pasted = text;
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		public static void RenderMenu(bool window = false) {
			if (window ? ImGui.BeginPopupContextWindow("window_node_menu", 1) : ImGui.BeginPopupContextItem("node_menu", 1)) {
				
				if (ImGui.Selectable("Copy (Ctrl+C)")) {
					Copy();
				}

				RenderPaste();
				
				if (ImGui.Selectable("Delete (Delete)")) {
					CurrentMenu.Remove();
				}
				
				ImGui.EndPopup();
			}
		}

		public static ImNode CurrentActive;
	}
}