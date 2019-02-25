using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class UsableProp : SolidProp {
		protected bool Used;

		public bool Use() {
			return true;
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Used = Reader.ReadBoolean();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Used);
		}
	}
}