using Pico8Emulator.unit.mem;
using System;

namespace Pico8Emulator.unit.audio {
	public class MusicPlayer {
		private PatternData[] _patternData;
		private Sfx[] _channels;
		private int _patternIndex;
		private Emulator _emulator;

		private Sfx _referenceSfx;

		private Oscillator _oscillator;

		public bool IsPlaying { get; private set; }

		public MusicPlayer(Emulator emulator) {
			_emulator = emulator;
		}

		public void LoadMusic() {
			_channels = new Sfx[4] { null, null, null, null };
			IsPlaying = false;

			_oscillator = new Oscillator(AudioUnit.SampleRate);
			_patternData = new PatternData[64];

			for (int i = 0; i < _patternData.Length; i += 1) {
				byte[] vals = {
					_emulator.Memory.ram[i * 4 + 0 + RamAddress.Song],
					_emulator.Memory.ram[i * 4 + 1 + RamAddress.Song],
					_emulator.Memory.ram[i * 4 + 2 + RamAddress.Song],
					_emulator.Memory.ram[i * 4 + 3 + RamAddress.Song]
				};

				if ((vals[0] & 0x80) == 0x80) {
					_patternData[i].loopStart = true;
				}

				if ((vals[1] & 0x80) == 0x80) {
					_patternData[i].loopEnd = true;
				}

				if ((vals[2] & 0x80) == 0x80) {
					_patternData[i].shouldStop = true;
				}

				_patternData[i].channelCount = new ChannelData[4];

				for (int j = 0; j < 4; j += 1) {
					_patternData[i].channelCount[j] = new ChannelData();

					if ((vals[j] & 0b01000000) != 0) {
						_patternData[i].channelCount[j].isSilent = true;
					}

					_patternData[i].channelCount[j].sfxIndex = (byte)(vals[j] & 0b00111111);
				}
			}
		}

		public void Update() {
			if (!IsPlaying || _patternIndex > 63 || _patternIndex < 0) {
				return;
			}

			Process();

			if (IsPatternDone()) {
				if (_patternData[_patternIndex].shouldStop) {
					Stop();
					return;
				}

				if (_patternData[_patternIndex].loopEnd) {
					_patternIndex = FindClosestLoopStart(_patternIndex);
				}
				else {
					_patternIndex += 1;

					if (_patternIndex > 63) {
						Stop();
						return;
					}
				}

				SetUpPattern();
				Process();
			}
		}

		private int FindClosestLoopStart(int index) {
			for (int i = index; i >= 0; i -= 1) {
				if (_patternData[i].loopStart)
					return i;
			}

			return 0;
		}

		private void Process() {
			for (int i = 0; i < 4; i += 1) {
				if (_channels[i] == null)
					continue;

				if (!_channels[i].Update()) {
					_channels[i] = null;
					continue;
				}
			}
		}

		private bool IsPatternDone() {
			if (_referenceSfx != null && !_referenceSfx.IsAlive)
				return true;

			return false;
		}

		private void SetUpPattern() {
			bool areAllLooping = true;
			Sfx longest = null;
			Sfx longestNoLoop = null;
			int audioBufferIndex = _referenceSfx?.AudioBufferIndex ?? 0;

			for (int i = 0; i < 4; i += 1) {
				if (_patternData[_patternIndex].channelCount[i].isSilent) {
					_channels[i] = null;
					continue;
				}

				byte[] _channelsData = new byte[68];
				Buffer.BlockCopy(_emulator.Memory.ram, RamAddress.Sfx + 68 * _patternData[_patternIndex].channelCount[i].sfxIndex, _channelsData, 0, 68);

				_channels[i] = new Sfx(_channelsData, _patternData[_patternIndex].channelCount[i].sfxIndex, ref _emulator.Audio.audioBuffer, ref _oscillator,
					AudioUnit.SampleRate, audioBufferIndex);

				_channels[i].Start();

				if (!_channels[i].HasLoop()) {
					areAllLooping = false;

					if (longestNoLoop == null || longestNoLoop.duration < _channels[i].duration)
						longestNoLoop = _channels[i];
				}

				if (longest == null || longest.duration < _channels[i].duration)
					longest = _channels[i];
			}

			_referenceSfx = areAllLooping ? longest : longestNoLoop;
			_referenceSfx.endLoop = _referenceSfx.startLoop;
		}

		public void Start(int n) {
			IsPlaying = true;
			_patternIndex = n;

			SetUpPattern();
		}

		public void Stop() {
			IsPlaying = false;
		}
	}
}