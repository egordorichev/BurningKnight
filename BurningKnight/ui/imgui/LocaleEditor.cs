using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Lens.assets;
using Lens.util.file;

namespace BurningKnight.ui.imgui {
	public static unsafe class LocaleEditor {
		private static ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 300);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(220, 10);
		private static bool filterByKey = true;
		private static System.Numerics.Vector2 spacer = new System.Numerics.Vector2(4, 1);
		private static List<ModifiedInfo> modified = new List<ModifiedInfo>();
		private static string[] aviableLocales;
		private static int locale;
		
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
			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Locale editor")) {
				ImGui.End();
				return;
			}

			if (ImGui.Combo("Locale", ref locale, aviableLocales, aviableLocales.Length)) {
				Locale.Load(aviableLocales[locale]);	
			}
			
			ImGui.Separator();
			
			filter.Draw("");
			ImGui.SameLine();
			ImGui.Checkbox("By key", ref filterByKey);
			
			ImGui.Separator();
			var height = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
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

			if (modified.Count > 0) {
				foreach (var t in modified) {
					if (t.KeyChanged) {
						Locale.Map.Remove(t.OldKey);
						Locale.Map[t.Key] = t.Value;
					} else {
						Locale.Map.Remove(Locale.Map.FirstOrDefault(m => m.Value == t.OldValue).Key);
						Locale.Map[t.Key] = t.Value;
					}
				}
				
				modified.Clear();
			}

			ImGui.PopStyleVar();
			ImGui.EndChild();
			ImGui.Separator();
			
			ImGui.End();
		}
	}
}