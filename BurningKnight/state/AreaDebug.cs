using BurningKnight.assets.particle;
using BurningKnight.entity;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.rooms;
using ImGuiNET;
using Lens.entity;
using Vector2 = System.Numerics.Vector2;

namespace BurningKnight.state {
	public static unsafe class AreaDebug {
		private static ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static bool hideLevel = true;
		private static bool onlyOnScreen;

		public static bool PassFilter(Entity e) {
			return !(e is Level || e is RenderTrigger || e is DestroyableLevel || e is Chasm || e is Room || e is ParticleEntity || e is ParticleSystem || e is ChasmFx);
		}

		public static Entity ToFocus;
		
		public static void Render(Area area) {
			if (ToFocus != null) {
				ImGui.SetNextWindowCollapsed(false);
			}
			
			if (!ImGui.Begin("Entities")) {
				ImGui.End();
				return;
			}			
			
			ImGui.Text($"Total {area.Entities.Entities.Count} entities");
			filter.Draw();

			ImGui.Checkbox("Hide level helpers", ref hideLevel);
			ImGui.Checkbox("Show only on screen", ref onlyOnScreen);
			
			var collapse = ImGui.Button("Collapse all");
			
			ImGui.Separator();
			
			Vector2 pos;
			Vector2 size;
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionConsole", new Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var e in area.Entities.Entities) {
				var type = e.GetType().FullName;
				
				if (filter.PassFilter(type) && (!onlyOnScreen || e.OnScreen) && (!hideLevel || PassFilter(e))) {
					if (ToFocus == e) {
						ImGui.SetScrollHereY();
						ImGui.SetNextTreeNodeOpen(true);
						ToFocus = null;
					} else if (collapse) {
						ImGui.SetNextTreeNodeOpen(false);
					}
					
					if (ImGui.CollapsingHeader($"{type}##{e.GetHashCode()}")) {
						ImGui.PushID(e.GetHashCode());
						
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
						
						e.RenderImDebug();
						
						ImGui.Separator();
						
						ImGui.Text("Components");
						
						foreach (var component in e.Components) {
							if (ImGui.TreeNode(component.Key.Name)) {
								component.Value.RenderDebug();
								ImGui.TreePop();
							}
						}
						
						ImGui.PopID();
					}
				}
			}
			
			ImGui.EndChild();
			ImGui.End();
		}
	}
}