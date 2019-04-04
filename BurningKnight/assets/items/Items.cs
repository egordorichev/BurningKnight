using Lens.lightJson;
using Lens.util.file;

namespace BurningKnight.assets.items {
	public static class Items {
		public static void Load() {
			Load(FileHandle.FromRoot("Items/"));
		}

		private static void Load(FileHandle handle) {
			if (!handle.Exists()) {
				return;
			}

			if (handle.IsDirectory()) {
				foreach (var file in handle.ListFileHandles()) {
					Load(file);
				}

				foreach (var file in handle.ListDirectoryHandles()) {
					Load(file);
				}

				return;
			}
			
			if (handle.Extension != ".json") {
				return;
			}
			
			var root = JsonValue.Parse(handle.ReadAll());

			foreach (var item in root.AsJsonObject) {
				ParseItem(item.Key, item.Value);
			}
		}

		private static void ParseItem(string id, JsonValue item) {
			var data = new ItemData {
				Id = id,
				Data = item
			};
		}
		
		public static void Destroy() {
			
		}
	}
}