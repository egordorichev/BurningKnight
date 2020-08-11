using Lens.entity.component;
using Lens.util.file;

namespace Lens.input {
	public class InputComponent : SaveableComponent {
		public byte Index;
		public bool GamepadEnabled = true;
		public bool KeyboardEnabled = true;

		public GamepadData GamepadData;
		
		public override void Load(FileReader stream) {
			base.Load(stream);

			Index = stream.ReadByte();
			GamepadEnabled = stream.ReadBoolean();
			KeyboardEnabled = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(Index);
			stream.WriteBoolean(GamepadEnabled);
			stream.WriteBoolean(KeyboardEnabled);
		}
	}
}