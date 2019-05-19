using BurningKnight.entity;
using BurningKnight.level;
using ImGuiNET;
using Lens.entity;
using Vector2 = System.Numerics.Vector2;

namespace BurningKnight.state {
	public static unsafe class AreaDebug {
		private static ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static bool hideLevel = true;

		public static bool PassFilter(Entity e) {
			return !(e is Level || e is RenderTrigger || e is DestroyableLevel || e is Chasm);
		}

		public static Entity ToFocus;
		
		public static void Render(Area area) {
			if (ToFocus != null) {
				ImGui.SetNextWindowCollapsed(false);
			}
			
			if (!ImGui.Begin("Entities", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}			
			
			ImGui.Text($"Total {area.Entities.Entities.Count} entities");
			filter.Draw();

			ImGui.Checkbox("Hide level helpers", ref hideLevel);
			ImGui.Separator();
			
			Vector2 pos;
			Vector2 size;

			foreach (var e in area.Entities.Entities) {
				var type = e.GetType().FullName;
				
				if (filter.PassFilter(type) && (!hideLevel || PassFilter(e))) {
					if (ToFocus == e) {
						ImGui.SetScrollHereY();
						ImGui.SetNextTreeNodeOpen(true);
						ToFocus = null;
					}
					
					if (ImGui.CollapsingHeader(type)) {
						pos.X = e.X;
						pos.Y = e.Y;

						if (ImGui.DragFloat2("Position", ref pos)) {
							e.X = pos.X;
							e.Y = pos.Y;
						}
						
						size.X = e.Width;
						size.Y = e.Height;

						if (ImGui.DragFloat2("Size", ref size)) {
							e.Width = size.X;
							e.Height = size.Y;
						}

						ImGui.InputInt("Depth", ref e.Depth);

						ImGui.Checkbox("Always active", ref e.AlwaysActive);
						ImGui.SameLine();
						ImGui.Checkbox("Always visible", ref e.AlwaysVisible);
						ImGui.Checkbox("On screen", ref e.OnScreen);
						ImGui.Checkbox("Done", ref e.Done);
						
						ImGui.Text("Components");
						
						foreach (var component in e.Components) {
							if (ImGui.TreeNode(component.Key.Name)) {
								component.Value.RenderDebug();
								ImGui.TreePop();
							}
						}
					}
				}
			}

			ImGui.End();
		}
	}
}