using System;
using System.Text;
using Lens.graphics;
using Lens.util.file;

namespace BurningKnight.entity.item {
	public class Item : SaveableEntity {
		public static TextureRegion Missing = Graphics.GetTexture("item-missing");

		private int count = 1;

		public int Count {
			get => count;
			set {
				count = Math.Max(0, value);

				if (count == 0) {
					Done = true;
				}
			}
		}
		
		public bool Stackable = false;
		public bool Usable = true;

		public override void Save(FileWriter stream) {
			stream.WriteInt32(Count);
		}

		public override void Load(FileReader stream) {
			Count = stream.ReadInt32();
		}

		public StringBuilder BuildInfo() {
			var builer = new StringBuilder();
			builer.Append();

			if (!Description.IsEmpty()) {
				builer.Append('\n');
				builer.Append(GetDescription());
			}

			return builer;
		}
	}
}