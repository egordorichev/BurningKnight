using BurningKnight.entity.level;
using BurningKnight.util.geometry;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class Spawn : SaveableEntity {
		public static Spawn Instance;
		public Rect Room = new Rect();

		public override void Init() {
			base.Init();
			Instance = this;
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Room.Left = Reader.ReadInt16();
			Room.Top = Reader.ReadInt16();
			Room.Resize(Reader.ReadInt16(), Reader.ReadInt16());
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) Room.Left);
			Writer.WriteInt16((short) Room.Top);
			Writer.WriteInt16((short) Room.GetWidth());
			Writer.WriteInt16((short) Room.GetHeight());
		}

		public override void Destroy() {
			base.Destroy();
			Instance = null;
		}
	}
}