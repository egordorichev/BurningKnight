using System.Collections.Generic;
using ImGuiNET;
using Lens.util.file;

namespace BurningKnight.state.save {
	public unsafe class EntityInspector : SaveInspector {
		public Dictionary<string, EntityData> Datas = new Dictionary<string, EntityData>();
		public int total;
		
		public override void Inspect(FileReader reader) {
			var count = reader.ReadInt32();
			var lastType = "";

			for (var i = 0; i < count; i++) {
				var type = reader.ReadString();

				if (type == null) {
					type = lastType;
				}

				if (!Datas.TryGetValue(type, out var data)) {
					data = new EntityData();
					Datas[type] = data;
				}

				data.Count++;
				total++;

				// It's important to split this into two expression statements
				// Because readInt affects position
				var size = reader.ReadInt16();
				reader.Position += size;
				lastType = type;
			}
		}
		
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		
		public override void Render() {
			ImGui.Text($"Entity Save ({total} entities)");
			filter.Draw();

			foreach (var pair in Datas) {
				if (filter.PassFilter(pair.Key)) {
					ImGui.BulletText($"{pair.Key} x{pair.Value.Count}");
				}
			}
		}
	}
}