using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class UsableProp : SolidProp {
		protected bool Used;

		public bool Use() {
			return true;
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Used = Reader.ReadBoolean();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Used);
		}
	}
}
