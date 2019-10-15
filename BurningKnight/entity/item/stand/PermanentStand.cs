using BurningKnight.assets.items;
using BurningKnight.entity.events;
using ImGuiNET;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.item.stand {
	public class PermanentStand : ItemStand {
		protected string SavedItem;

		public override void PostInit() {
			base.PostInit();

			if (SavedItem == null) {
				return;
			}
			
			if (Item != null) {
				if (Item.Id == SavedItem) {
					return;
				}
				
				Item.Done = true;
			}

			debugItem = SavedItem;
			SetItem(Items.CreateAndAdd(SavedItem, Area), null);
		}

		public override void SetItem(Item i, Entity entity, bool remove = true) {
			base.SetItem(i, entity, remove);
			Item?.CheckMasked();
		}

		public override void RenderImDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				if (Item != null) {
					Item.Done = true;
				}

				SetItem(Items.CreateAndAdd(debugItem, Area), null);
				SavedItem = debugItem;
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(SavedItem);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			SavedItem = stream.ReadString();
		}
	}
}