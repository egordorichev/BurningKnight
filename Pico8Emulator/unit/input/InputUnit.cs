using Pico8Emulator.lua;
using System;

namespace Pico8Emulator.unit.input {
	public class InputUnit : Unit {
		public const int ButtonCount = 6;
		public const int MaxPlayers = 8;
		public const int StateSize = ButtonCount * MaxPlayers;

		private bool[] _previousButtonState = new bool[StateSize];
		private bool[] _currentButtonState = new bool[StateSize];

		public InputUnit(Emulator emulator) : base(emulator) {

		}

		public override void DefineApi(LuaInterpreter script) {
			base.DefineApi(script);

			script.AddFunction("btn", (Func<int?, int?, object>)Btn);
			script.AddFunction("btnp", (Func<int?, int?, object>)Btnp);
		}

		public override void Update() {
			base.Update();

			for (var i = 0; i < ButtonCount; ++i) {
				for (var p = 0; p < MaxPlayers; ++p) {
					var index = ToIndex(i, p);
					_previousButtonState[index] = _currentButtonState[index];
					_currentButtonState[index] = Emulator.InputBackend.IsButtonDown(i, p);
				}
			}
		}

		private static int ToIndex(int? i, int? p) {
			return (i ?? 0) + ((p ?? 0) * ButtonCount);
		}

		public object Btn(int? i = null, int? p = null) {
			if (i == null) {
				int bitMask = 0;
				for (int k = 0; k < ButtonCount; ++k) {
					for (int j = 0; j < 2; ++j) {
						bitMask |= ((_currentButtonState[ToIndex(k, j)] ? 1 : 0) << (ButtonCount * j + k));
					}
				}
				return bitMask;
			}

			return _currentButtonState[ToIndex(i, p)];
		}

		public object Btnp(int? i = null, int? p = null) {
			int index = 0;

			if (i == null) {
				int bitMask = 0;
				for (int k = 0; k < ButtonCount; ++k) {
					for (int j = 0; j < 2; ++j) {
						index = ToIndex(k, j);
						bitMask |= ((_currentButtonState[index] && !_previousButtonState[index] ? 1 : 0) << (ButtonCount * j + k));
					}
				}
				return bitMask;
			}

			index = ToIndex(i, p);
			return _currentButtonState[index] && !_previousButtonState[index];
		}
	}
}