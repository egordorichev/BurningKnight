using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.util;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.util.file;

namespace BurningKnight.entity {
	public class Sign : SaveableEntity {
		public string Label;
		public Direction Direction = Direction.Center;

		private string localeLabel;
		
		public string LocaleLabel {
			set {
				localeLabel = value;
				Label = Locale.Get(value);
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) Direction.Convert());

			if (localeLabel != null) {
				stream.WriteBoolean(true);
				stream.WriteString(localeLabel);
			} else {
				stream.WriteString(Label);
				stream.WriteBoolean(false);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Direction = Directions.Convert(stream.ReadByte());

			if (stream.ReadBoolean()) {
				LocaleLabel = stream.ReadString();
			} else {
				Label = stream.ReadString();
			}
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new SliceComponent("props", $"sign_{Directions.ToString(Direction)}"));
		}
	}
}