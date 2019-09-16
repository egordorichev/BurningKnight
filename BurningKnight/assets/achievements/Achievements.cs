using System.Collections.Generic;
using System.IO;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.save;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.input;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.assets.achievements {
	public static class Achievements {
		private static Dictionary<string, Achievement> defined = new Dictionary<string, Achievement>();
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static System.Numerics.Vector2 popupSize = new System.Numerics.Vector2(400, 400);

		public static Achievement Get(string id) {
			return defined.TryGetValue(id, out var a) ? a : null;
		}
		
		public static void Load() {
			Load(FileHandle.FromRoot("achievements.json"));
		}

		private static void Load(FileHandle handle) {
			if (!handle.Exists()) {
				Log.Error($"Achievement data {handle.FullPath} does not exist!");
				return;
			}
			
			var root = JsonValue.Parse(handle.ReadAll());

			foreach (var item in root.AsJsonObject) {
				var a = new Achievement(item.Key);
				a.Load(item.Value);
				defined[item.Key] = a;
			}
		}

		public static void Save() {
			var root = new JsonObject();

			foreach (var a in defined.Values) {
				var data = new JsonObject();
				a.Save(data);
				root[a.Id] = data;
			}
			
			var file = File.CreateText(FileHandle.FromRoot("achievements.json").FullPath);
			var writer = new JsonWriter(file);
			writer.Write(root);
			file.Close();

			Locale.Save();
		}

		public static void LockAll() {
			foreach (var a in defined.Values) {
				a.Unlocked = false;
			}
		}

		public static void LoadState() {
			foreach (var a in defined.Values) {
				a.Unlocked = GlobalSave.IsTrue($"ach_{a.Id}");
			}
		}

		public static void Unlock(string id) {
			var a = Get(id);

			if (a == null) {
				Log.Error($"Unknown achievement {id}!");
				return;
			}

			if (a.Unlocked) {
				return;
			}

			a.Unlocked = true;
			GlobalSave.Put($"ach_{a.Id}", true);
			
			Log.Info($"Achievement {id} was complete!");

			var e = new Achievement.UnlockedEvent {
				Achievement = a
			};
			
			Engine.Instance.State.Area.EventListener.Handle(e);
			Engine.Instance.State.Ui.EventListener.Handle(e);
		}

		public static void Lock(string id) {
			var a = Get(id);

			if (a == null) {
				Log.Error($"Unknown achievement {id}!");
				return;
			}

			if (!a.Unlocked) {
				return;
			}

			a.Unlocked = false;
			GlobalSave.Put($"ach_{a.Id}", false);
			
			Log.Info($"Achievement {id} was locked!");

			var e = new Achievement.LockedEvent {
				Achievement = a
			};
			
			Engine.Instance.State.Area.EventListener.Handle(e);
			Engine.Instance.State.Ui.EventListener.Handle(e);
		}
		
		private static string achievementName = "";
		private static Achievement selected;
		private static bool hideLocked;
		private static bool hideUnlocked;
		private static bool forceFocus;

		private static void RenderSelectedInfo() {
			var open = true;
			
			if (!ImGui.Begin("Achievement", ref open, ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			if (!open) {
				selected = null;
				ImGui.End();

				return;
			}
			
			ImGui.Text(selected.Id);
			ImGui.Separator();
			
			var u = selected.Unlocked;
			
			if (ImGui.Checkbox("Unlocked", ref u)) {
				if (u) {
					Unlock(selected.Id);
				} else {
					Lock(selected.Id);
				}			
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Delete##ach")) {
				defined.Remove(selected.Id);
				selected = null;

				ImGui.End();
				return;
			}
			
			ImGui.Separator();

			var k = $"ach_{selected.Id}";
			var name = Locale.Get(k);
			
			if (ImGui.InputText("Name##ac", ref name, 64)) {
				Locale.Map[k] = name;
			}

			var key = $"ach_{selected.Id}_desc";
			var desc = Locale.Get(key);
				
			if (ImGui.InputText("Description##ac", ref desc, 256)) {
				Locale.Map[key] = desc;
			}

			ImGui.End();
		}
		
		public static void RenderDebug() {
			if (!WindowManager.Achievements) {
				return;
			}

			if (selected != null) {
				RenderSelectedInfo();
			}
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Achievements")) {
				ImGui.End();
				return;
			}

			if (ImGui.Button("New")) {
				ImGui.OpenPopup("New achievement");
			}

			ImGui.SameLine();

			if (ImGui.Button("Save")) {
				Log.Info("Saving achievements");
				Save();
			}

			if (ImGui.BeginPopupModal("New achievement")) {
				ImGui.PushItemWidth(300);
				ImGui.InputText("Id", ref achievementName, 64);
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Create") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					defined[achievementName] = selected = new Achievement(achievementName);
					achievementName = "";
					forceFocus = true;
					
					ImGui.CloseCurrentPopup();
				}	
				
				ImGui.SameLine();

				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					achievementName = "";
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
			
			ImGui.Separator();
			filter.Draw("Search");

			ImGui.Checkbox("Hide unlocked", ref hideUnlocked);
			ImGui.SameLine();
			ImGui.Checkbox("Hide locked", ref hideLocked);
			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in defined.Values) {
				ImGui.PushID(i.Id);

				if (forceFocus && i == selected) {
					ImGui.SetScrollHereY();
					forceFocus = false;
				}
				
				if (filter.PassFilter(i.Id)) {
					if ((hideLocked && !i.Unlocked) || (hideUnlocked && i.Unlocked)) {
						continue;
					}
					
					if (ImGui.Selectable(i.Id, i == selected)) {
						selected = i;

						if (ImGui.IsMouseDown(1)) {
							if (ImGui.Button("Give")) {
								LocalPlayer.Locate(Engine.Instance.State.Area)
									?.GetComponent<InventoryComponent>()
									.Pickup(Items.CreateAndAdd(
										selected.Id, Engine.Instance.State.Area
									));
							}
						}
					}
				}

				ImGui.PopID();
			}

			ImGui.EndChild();
			ImGui.End();
		}
	}
}