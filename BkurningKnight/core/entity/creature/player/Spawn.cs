using BurningKnight.core.entity.level;
using BurningKnight.core.util.file;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.player {
	public class Spawn : SaveableEntity {
		public static Spawn Instance;
		public Rect Room = new Rect();

		public override Void Init() {
			base.Init();
			Instance = this;
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Room.Left = Reader.ReadInt16();
			Room.Top = Reader.ReadInt16();
			Room.Resize(Reader.ReadInt16(), Reader.ReadInt16());
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) Room.Left);
			Writer.WriteInt16((short) Room.Top);
			Writer.WriteInt16((short) Room.GetWidth());
			Writer.WriteInt16((short) Room.GetHeight());
		}

		public override Void Destroy() {
			base.Destroy();
			Instance = null;
		}
	}
}
