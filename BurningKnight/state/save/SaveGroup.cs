using System.Collections.Generic;
using ImGuiNET;

namespace BurningKnight.state.save {
	public class SaveGroup : SaveNode {
		public List<SaveGroup> Dirs = new List<SaveGroup>();
		public List<SaveNode> Files = new List<SaveNode>();

		public override void Render() {
			foreach (var f in Dirs) {
				if (ImGui.TreeNode(f.Name)) {
					ImGui.TreePush();
					f.Render();
					ImGui.TreePop();
					ImGui.TreePop();
				}
			}
			
			foreach (var f in Files) {
				if (ImGui.TreeNode(f.Name)) {
					f.Render();
					ImGui.TreePop();
				}
			}
		}
	}
}