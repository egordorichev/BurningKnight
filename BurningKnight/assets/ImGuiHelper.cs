using System;
using System.Collections.Generic;
using BurningKnight.ui.imgui;
using BurningKnight.ui.imgui.node;
using ImGuiNET;
using Lens;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

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
		private static Vector2 size = new Vector2(200, 400);
		private static Color gridColor = new Color(0.15f, 0.15f, 0.15f, 1f);
		private static int gridSize = 128;
		private static Vector2? target;
		private static bool grid = true;
		
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
			if (grid) {
				var list = ImGui.GetBackgroundDrawList();

				var width = Engine.Instance.GetScreenWidth();
				var height = Engine.Instance.GetScreenHeight();
				var off = ImNode.Offset;

				for (float x = off.X % gridSize; x <= width - off.X % gridSize; x += gridSize) {
					list.AddLine(new Vector2(x, 0), new Vector2(x, height), gridColor.PackedValue);
				}

				for (float y = off.Y % gridSize; y <= height - off.Y % gridSize; y += gridSize) {
					list.AddLine(new Vector2(0, y), new Vector2(width, y), gridColor.PackedValue);
				}
			}

			RenderVoidMenu();

			if (target.HasValue) {
				ImNode.Offset += (target.Value - ImNode.Offset) * Engine.Delta * 10f;

				if ((target.Value - ImNode.Offset).Length() <= 3f) {
					target = null;
				}
			} else if (ImGui.IsMouseDragging(2)) {
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

			ImGui.Checkbox("Show grid", ref grid);

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

					target =
						-node.RealPosition + new Vector2((Engine.Instance.GetScreenWidth() - node.Size.X) / 2,
							(Engine.Instance.GetScreenHeight() - node.Size.Y) / 2);
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