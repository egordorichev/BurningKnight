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
		private static Vector2 defaultSize = new Vector2(400, 700);
		private static ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static bool hideLevel = true;
		private static bool onlyOnScreen;
		private static Vector2 pos;
		private static Vector2 size;

		public static bool PassFilter(Entity e) {
			return !(e is Level || e is RenderTrigger || e is DestroyableLevel || e is Chasm || e is Room || e is ParticleEntity || e is ParticleSystem || e is ChasmFx);
		}

		public static Entity ToFocus;
		
		public static void Render(Area area) {
			if (ToFocus != null) {
				ImGui.SetNextWindowCollapsed(false);
			}

			ImGui.SetNextWindowSize(defaultSize, ImGuiCond.Once);
			
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
			
			var height = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing() + 4;
			ImGui.BeginChild("ScrollingRegionConsole", new Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var e in (hasTag ? area.Tags[BitTag.Tags[currentTag]] : area.Entities.Entities)) {
				if (filter.PassFilter(e.GetType().FullName) && (!onlyOnScreen || e.OnScreen) && (!hideLevel || PassFilter(e))) {
					if (ToFocus == e) {
						ImGui.SetScrollHereY();
						ImGui.SetNextTreeNodeOpen(true);
						ToFocus = null;
					} else if (collapse) {
						ImGui.SetNextTreeNodeOpen(false);
					}

					RenderEntity(e);
				}
			}
			
			ImGui.EndChild();
			ImGui.Separator();

			ImGui.Checkbox("Must have tag", ref hasTag);

			if (hasTag) {
				ImGui.SameLine();
				ImGui.Combo("Tag", ref currentTag, Tags.AllTags, Tags.AllTags.Length);
			}
			
			ImGui.End();
		}

		private static int currentTag;
		private static bool hasTag;
		
		public static void RenderEntity(Entity e) {
			if (ImGui.CollapsingHeader($"{e.GetType().FullName}##{e.GetHashCode()}")) {
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
				ImGui.TreePush();
				
				if (ImGui.CollapsingHeader("Components")) {
					foreach (var component in e.Components) {
						if (ImGui.TreeNode(component.Key.Name)) {
							component.Value.RenderDebug();
							ImGui.TreePop();
						}
					}
				}
				
				if (ImGui.CollapsingHeader("Tags")) {
					for (var i = 0; i < BitTag.Total; i++) {
						if (e.HasTag(BitTag.Tags[i])) {
							ImGui.BulletText(BitTag.Tags[i].Name);
						}
					}
				}

				ImGui.TreePop();
				ImGui.Separator();
				ImGui.PopID();
			}
		}
	}
}