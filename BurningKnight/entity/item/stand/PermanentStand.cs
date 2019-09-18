using BurningKnight.assets.items;
using ImGuiNET;

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
			
			SetItem(Items.CreateAndAdd(item, Area), null);
		}

		public override void RenderDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				SetItem(Items.CreateAndAdd(debugItem, Area), null);
				item = debugItem;
			}
		}
	}
}