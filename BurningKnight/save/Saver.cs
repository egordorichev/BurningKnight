using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public abstract class Saver {
		public abstract string GetPath(string path, bool old = false);
		public abstract void Load(Area area, FileReader reader);
		public abstract void Save(Area area, FileWriter writer);
		public abstract void Generate(Area area);

		public virtual FileHandle GetHandle() {
			return new FileHandle(GetPath(SaveManager.SlotDir));
		}

		public virtual void Delete() {
			var handle = GetHandle();

			if (handle.Exists()) {
				handle.Delete();
			}
		}
	}
}