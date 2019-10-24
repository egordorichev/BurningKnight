using Pico8Emulator.lua;

namespace Pico8Emulator.unit {
	public class Unit {
		protected Emulator Emulator;

		public Unit(Emulator emulator) {
			Emulator = emulator;
		}

		public virtual void DefineApi(LuaInterpreter script) {

		}

		public virtual void Init() {

		}

		public virtual void Destroy() {

		}

		public virtual void Update() {

		}

		public virtual void OnCartridgeLoad() {

		}
	}
}