using System;
using ImGuiNET;
using Lens.util.file;

namespace BurningKnight.state.save {
	public class GameInspector : SaveInspector {
		private sbyte depth;
		private int killCount;
		private float time;
		private int id;
		private string seed;

		public override void Inspect(FileReader reader) {
			depth = reader.ReadSbyte();
			killCount = reader.ReadInt32();
			time = reader.ReadFloat();
			id = reader.ReadInt32();
			seed = reader.ReadString();
		}

		public override void Render() {
			ImGui.Text("Game Save");

			ImGui.Text($"Depth: {depth}");
			ImGui.Text($"Kill count: {killCount}");
			ImGui.Text($"Time: {Math.Floor(time / 360)}:{Math.Floor(time / 60)}:{(int) Math.Floor(time % 1) * 60}");
			ImGui.Text($"Id: {id}");
			ImGui.Text($"Seed: {seed}");
		}
	}
}