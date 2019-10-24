using BurningKnight.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.door {
	public class LevelDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new LevelLock();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			SkipLock = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(!TryGetComponent<LockComponent>(out var c) || !c.Lock.IsLocked);
		}
	}
}