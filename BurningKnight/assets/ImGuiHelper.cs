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
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(200, 400);

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
			
			if (ImGui.IsMouseDragging(2)) {
				ImNode.Offset += ImGui.GetIO().MouseDelta;
			}
			
			foreach (var n in Nodes) {
				var node = n.Value;
				
				if (!hideFiltred || filter.PassFilter(node.GetName())) {
					node.Render();
				}

				if (node.Done) {
					toRemove.Add(n.Key);
				}
			}

			CurrentActive?.RemoveEmptyConnection();

			if (toRemove.Count > 0) {
				foreach (var k in toRemove) {
					Nodes.Remove(k);
				}
				
				toRemove.Clear();
			}
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Nodes")) {
				ImGui.End();
				return;
			}

			filter.Draw("Filter");
			ImGui.Checkbox("Hide filtred", ref hideFiltred);
			ImGui.Separator();
			
			RenderMenu();

			ImNode first = null;
			var sawFocused = false;
			
			foreach (var p in Nodes) {
				var node = p.Value;
				var name = node.GetName();

				if (!filter.PassFilter(name)) {
					continue;
				}

				if (first == null) {
					first = node;
				}

				if (ImNode.Focused == node) {
					node.ForceFocus = true;
					sawFocused = true;
				}
				
				if (ImGui.Selectable($"{name} #{node.Id}", ImNode.Focused == node)) {
					ImNode.Focused = node;
					node.ForceFocus = true;
					sawFocused = true;
				}

				if (ImGui.OpenPopupOnItemClick("node_menu", 1)) {
					CurrentMenu = node;
				}
			}

			if (!sawFocused) {
				ImNode.Focused = first;
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
		private static bool hideFiltred;

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
				
				if (Input.Keyboard.WasPressed(Keys.D, true) && ImNode.Focused != null) {
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
				
				if (ImGui.Selectable("Delete (Ctrl+D)")) {
					CurrentMenu.Remove();
				}
				
				ImGui.EndPopup();
			}
		}

		public static ImNode CurrentActive;
	}
}