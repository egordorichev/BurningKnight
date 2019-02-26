using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Chair : Prop {
		public bool Flipped;

		public override void Init() {
			Sprite = Flipped ? "props-chair_a" : "props-char_b";
			base.Init();
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Region = Graphics.GetTexture(Sprite);
			Flipped = Reader.ReadBoolean();
			Sprite = Flipped ? "props-chair_a" : "props-chair_b";
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Flipped);
		}
	}
}