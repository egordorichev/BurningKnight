using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurningKnight.assets;
using BurningKnight.ui.imgui.node;
using BurningKnight.ui.str;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.imgui {
	public static class DialogEditor {
		private static string[] files;
		private static int current;

		public static string Current => files[current];
		
		public static void Init() {
			Load();
			LoadCurrent();

			str = new UiString(Font.Small);
			Engine.Instance.State.Ui.Add(str);
			str.Position = new Vector2(5);
		}

		public static void Destroy() {
			SaveDialogs();
			ImGuiHelper.ClearNodes();
		}
	
		private static void SaveDialogs() {
			if (files.Length == 0) {
				return;
			}
			
			Locale.Save();
			Log.Info($"Saving {files[current]}");
			
			var nodes = ImGuiHelper.Nodes;
			var root = new JsonArray();

			foreach (var p in nodes) {
				var node = p.Value;
				
				var obj = new JsonObject();

				node.Save(obj);
				root.Add(obj);
			}

			var file = File.CreateText(FileHandle.FromRoot($"Dialogs/{files[current]}.json").FullPath);
			var writer = new JsonWriter(file);
			writer.Write(root);
			file.Close();
		}

		public static void Load() {
			var folder = FileHandle.FromRoot("Dialogs");

			if (!folder.Exists()) {
				Log.Error("Dialog folder is not found!");
				files = new string[0];
				return;
			}

			var fs = new List<string>();
			
			foreach (var f in folder.ListFileHandles()) {
				if (f.Extension == ".json") {
					fs.Add(f.NameWithoutExtension);
				}
			}

			files = fs.ToArray();
		}
		
		public static void LoadCurrent() {
			if (files.Length == 0) {
				return;
			}
			
			try {
				ImGuiHelper.ClearNodes();
				var name = files[current];
				LoadFromRoot(name, JsonValue.Parse(FileHandle.FromRoot($"Dialogs/{name}.json").ReadAll()));
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		private static void LoadFromRoot(string name, JsonArray root) {
			foreach (var vl in root) {
				ImNode.Create(name, vl);
			}
		}

		private static ImNode last;
		private static UiString str;

		public static void RenderUi() {
			if (ImNode.Focused != null) {
				if (last != ImNode.Focused) {
					last = ImNode.Focused;
					str.Label = Locale.Get(ImNode.Focused.LocaleId);
				}
			}
			
			str.Render();
		}
		
		private static string newName = "";

		public static void Render() {
			if (ImGuiHelper.BeforeRender()) {
				ImGui.PushItemWidth(-1);

				var old = current;
				
				if (ImGui.Combo("##file", ref current, files, files.Length)) {
					var o = current;
					current = old;
					SaveDialogs();
					current = o;
					LoadCurrent();
				}
				
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Save")) {
					SaveDialogs();
				}
				
				ImGui.SameLine();
				
				if (ImGui.Button("Delete")) {
					ImGui.OpenPopup("Delete?");
				}
				
				if (ImGui.BeginPopupModal("Delete?")) {
					ImGui.Text("This operation can't be undone!");
					ImGui.Text("Are you sure?");
					
					if (ImGui.Button("Yes")) {
						ImGui.CloseCurrentPopup();
						var list = files.ToList();
						var s = files[current];

						try {
							var file = FileHandle.FromRoot($"Dialogs/{s}.json");
							file.Delete();
						} catch (Exception e) {
							Log.Error(e);
						}
						
						list.RemoveAt(current);
						files = list.ToArray();
						current = 0;
						LoadCurrent();
					}
						
					ImGui.SameLine();
					ImGui.SetItemDefaultFocus();
					
					if (ImGui.Button("No")) { 
						ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}
				
				ImGui.SameLine();
				
				if (ImGui.Button("New")) {
					ImGui.OpenPopup("New tree");
				}
			
				if (ImGui.BeginPopupModal("New tree")) {
					ImGui.SetItemDefaultFocus();
					var input = ImGui.InputText("Name", ref newName, 64, ImGuiInputTextFlags.EnterReturnsTrue);
					var button = ImGui.Button("Create");

					ImGui.SameLine();
				
					if (ImGui.Button("Cancel")) {
						ImGui.CloseCurrentPopup();
						newName = "";
					} else {
						if (input || button) {
							var list = files.ToList();
							list.Add(newName);
							files = list.ToArray();
							current = list.Count - 1;

							newName = "";
							SaveDialogs();
							LoadCurrent();
							ImGuiHelper.ClearNodes();
							ImGui.CloseCurrentPopup();
						}	
					}

					ImGui.EndPopup();
				}

				ImGui.Separator();
				ImGuiHelper.RenderNodes();
			}
		}
	}
}