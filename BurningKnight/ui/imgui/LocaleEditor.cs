using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Lens.assets;
using Lens.input;
using Lens.util.file;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.imgui {
	public static unsafe class LocaleEditor {
		private static ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(220, 10);
		private static bool filterByKey = true;
		private static System.Numerics.Vector2 spacer = new System.Numerics.Vector2(4, 1);
		private static List<ModifiedInfo> modified = new List<ModifiedInfo>();
		private static string[] aviableLocales;
		private static int locale;
		private static string newKey = "";
		private static string created;
		private static bool showEnglish = true;
		private static string newLocaleName = "";
		private static bool open;
		
		private class ModifiedInfo {
			public string OldKey;
			public string OldValue;
			public string Key;
			public string Value;
			public bool KeyChanged;
		}

		static LocaleEditor() {
			var locales = new FileHandle("Content/Locales/");

			if (!locales.Exists()) {
				aviableLocales = new[] { "en" };
				return;
			}

			var names = new List<string>();

			foreach (var f in locales.ListFileHandles()) {
				if (f.Extension == ".json") {
					names.Add(f.NameWithoutExtension);
				}
			}

			aviableLocales = names.ToArray();
			locale = names.IndexOf(Locale.Current);
		}
		
		public static void Render() {
			if (Input.Keyboard.WasPressed(Keys.F10)) {
				open = !open;
			}
			
			if (!open) {
				return;
			}
			
			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Locale editor")) {
				ImGui.End();
				return;
			}

			if (ImGui.Combo("##locale", ref locale, aviableLocales, aviableLocales.Length)) {
				Locale.Load(aviableLocales[locale]);	
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Save")) {
				Locale.Save();
			}

			ImGui.SameLine();
			
			if (ImGui.Button("New")) {
				ImGui.OpenPopup("New locale");
			}
			
			if (ImGui.BeginPopupModal("New locale")) {
				ImGui.SetItemDefaultFocus();
				var input = ImGui.InputText("Name", ref newLocaleName, 3, ImGuiInputTextFlags.EnterReturnsTrue);
				var button = ImGui.Button("Create");

				ImGui.SameLine();
				
				if (ImGui.Button("Cancel")) {
					ImGui.CloseCurrentPopup();
					newLocaleName = "";
				} else {
					if (input || button) {
						Locale.Current = newLocaleName;
						Locale.Map = new Dictionary<string, string>();
						Locale.Loaded[newLocaleName] = Locale.Map;
					
						var list = aviableLocales.ToList();
						list.Add(newLocaleName);
						aviableLocales = list.ToArray();
						locale = list.Count - 1;

						newLocaleName = "";
						ImGui.CloseCurrentPopup();
					}	
				}

				ImGui.EndPopup();
			}

			var notEng = aviableLocales[locale] != "en";

			if (notEng) {
				if (ImGui.Button("Clear")) {
					Locale.Map.Clear();
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
						var list = aviableLocales.ToList();
						list.Remove(Locale.Current);
						aviableLocales = list.ToArray();
						locale = 0;
						Locale.Delete();
					}
						
					ImGui.SameLine();
					ImGui.SetItemDefaultFocus();
					
					if (ImGui.Button("No")) { 
						ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}
			}

			if (notEng) {
				ImGui.SameLine();

				if (ImGui.Button("Add en")) {
					foreach (var t in Locale.Fallback) {
						if (!Locale.Map.ContainsKey(t.Key)) {
							Locale.Map[t.Key] = t.Value;
						}
					}
				}
			}

			ImGui.Text(notEng ? $"{Locale.Map.Count} entries (en has {Locale.Fallback.Count})" : $"{Locale.Map.Count} entries");

			if (notEng) {
				ImGui.Checkbox("Show english", ref showEnglish);
			}
			
			ImGui.Separator();
			
			filter.Draw("");
			ImGui.SameLine();
			ImGui.Checkbox("By key", ref filterByKey);
			
			ImGui.Separator();
			var height = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing() + 4;
			ImGui.BeginChild("ScrollingRegionLocale", new System.Numerics.Vector2(0, -height), 
				false);
			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, spacer);

			var i = 0;
			
			foreach (var t in Locale.Map) {
				if (filter.PassFilter(filterByKey ? t.Key : t.Value)) {
					var key = t.Key;
					var value = t.Value;

					ImGui.PushItemWidth(100);
					ImGui.PushID($"{i}__key");
					ImGui.InputText("", ref key, 64);
					ImGui.PopID();
					ImGui.PopItemWidth();
					ImGui.SameLine();
					ImGui.PushID($"{i}__value");
					ImGui.InputText("", ref value, 256);

					if (created == t.Key) {
						ImGui.SetKeyboardFocusHere(-1);
						ImGui.SetScrollHereY(-1);
						created = null;
					}

					if (notEng && showEnglish && Locale.Fallback.TryGetValue(t.Key, out var en)) {
						ImGui.BulletText(en);
					}

					if (key != t.Key || value != t.Value) {
						modified.Add(new ModifiedInfo {
							OldKey = t.Key,
							OldValue = t.Value,
							Key = key,
							Value = value,
							KeyChanged = key != t.Key
						});
					}
				}

				i++;
			}
			
			ImGui.PopStyleVar();
			ImGui.EndChild();
			ImGui.Separator();

			var enter = ImGui.InputText("##newkey", ref newKey, 128, ImGuiInputTextFlags.EnterReturnsTrue);
			ImGui.SameLine();
			
			if ((enter || ImGui.Button("Add")) && newKey.Length > 0) {
				modified.Add(new ModifiedInfo {
					Key = newKey,
					Value = newKey
				});

				created = newKey;
				newKey = "";
			}

			if (modified.Count > 0) {
				foreach (var t in modified) {
					if (t.OldKey != null) {
						Locale.Map.Remove(t.KeyChanged ? t.OldKey : Locale.Map.FirstOrDefault(m => m.Value == t.OldValue).Key);
					}

					if (t.Key.Length > 0) {
						Locale.Map[t.Key] = t.Value;
					}
				}
				
				modified.Clear();
			}

			ImGui.End();
		}
	}
}