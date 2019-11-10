using ImGuiNET;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class StatsComponent : Component {
		public float Speed = 1f;
		public float Damage = 1f;
		public float FireRate = 1f;
		public float Accuracy = 1f;
		public float Range = 1f;

		public override void RenderDebug() {
			base.RenderDebug();

			ImGui.InputFloat("Speed", ref Speed);
			ImGui.InputFloat("Damage", ref Damage);
			ImGui.InputFloat("Fire Rate", ref FireRate);
			ImGui.InputFloat("Accuracy", ref Accuracy);
			ImGui.InputFloat("Range", ref Range);
		}
	}
}