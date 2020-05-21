using BurningKnight.assets.items;
using BurningKnight.ui.editor;
using Lens.util.file;

namespace BurningKnight.entity.item {
	public class RandomItem : RoundItem {
		public bool Prevent;
	
		private static string GenerateId() {
			return Items.Generate(ItemPool.Treasure);
		}

		private void TryToOverrideId() {
			if (Prevent) {
				return;
			}
			
			Id = GenerateId();
		}

		public override void PostInit() {
			TryToOverrideId();
			base.PostInit();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Prevent = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Prevent);
		}
	}
}