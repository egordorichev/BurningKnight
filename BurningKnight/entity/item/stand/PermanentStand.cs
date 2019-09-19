using BurningKnight.assets.items;
using ImGuiNET;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.item.stand {
	public class PermanentStand : ItemStand {
		private string item;

		public override void PostInit() {
			base.PostInit();

			if (item == null) {
				return;
			}
			
			if (Item != null) {
				if (Item.Id == item) {
					return;
				}
				
				Item.Done = true;
			}
			
			Log.Error("Resetting " + item);
			SetItem(Items.CreateAndAdd(item, Area), null);
		}

		/*public override void RenderImDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				SetItem(Items.CreateAndAdd(debugItem, Area), null);
				item = debugItem;
			}
		}*/

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(item);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			item = stream.ReadString();
		}
	}
}