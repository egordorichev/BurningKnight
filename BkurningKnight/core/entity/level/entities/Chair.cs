using BurningKnight.core.assets;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Chair : Prop {
		public bool Flipped;

		public override Void Init() {
			this.Sprite = Flipped ? "props-chair_a" : "props-char_b";
			base.Init();
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Region = Graphics.GetTexture(this.Sprite);
			Flipped = Reader.ReadBoolean();
			this.Sprite = Flipped ? "props-chair_a" : "props-chair_b";
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Flipped);
		}
	}
}
