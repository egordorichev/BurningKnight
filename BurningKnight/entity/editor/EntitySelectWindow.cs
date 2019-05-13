using System;
using System.Collections.Generic;
using BurningKnight.entity.chest;
using BurningKnight.entity.creature.npc;
using ImGuiNET;

namespace BurningKnight.entity.editor {
	public unsafe class EntitySelectWindow {
		private static List<Type> types = new List<Type>();
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private int selected;

		public Editor Editor;
		public bool SnapToGrid;
		public bool Center;
		
		static EntitySelectWindow() {
			// todo: get all types in the namespace Burning Knight that extend entity
			types.Add(typeof(OldMan));
			types.Add(typeof(Ord));
			types.Add(typeof(Chest));
			types.Add(typeof(LockedChest));
			
			types.Sort((a, b) => a.GetType().FullName.CompareTo(b.GetType().FullName));
		}
		
		public EntitySelectWindow(Editor e) {
			Editor = e;
			Editor.Cursor.SetEntity(types[selected]);
		}
		
		public void Render() {
			ImGui.Checkbox("Snap to grid", ref SnapToGrid);
			ImGui.SameLine();
			ImGui.Checkbox("Center", ref Center);
						
			filter.Draw();
			var i = 0;
			
			foreach (var t in types) {
				if (filter.PassFilter(t.FullName)) {
					if (ImGui.Selectable(t.FullName, selected == i)) {
						selected = i;
						Editor.Cursor.SetEntity(types[selected]);
						Editor.Cursor.CurrentMode = EditorCursor.Mode.Entity;
					}
				}

				i++;
			}
		}	
	}
}