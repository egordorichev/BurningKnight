using Pico8Emulator.lua;
using Pico8Emulator.unit.mem;
using System;

namespace Pico8Emulator.unit.audio {
	public class AudioUnit : Unit {
		public const int SampleRate = 48000;
		public const int BufferSize = 2048;
		public const int ChannelCount = 4;

		public Sfx[] sfxChannels = new Sfx[ChannelCount];
		public float[] externalAudioBuffer = new float[BufferSize];
		public float[] audioBuffer = new float[BufferSize];

		private MusicPlayer _musicPlayer;

		public AudioUnit(Emulator emulator) : base(emulator) {
			_musicPlayer = new MusicPlayer(Emulator);
		}

		public override void OnCartridgeLoad() {
			_musicPlayer.LoadMusic();
		}

		public override void DefineApi(LuaInterpreter script) {
			base.DefineApi(script);

			script.AddFunction("music", (Action<int, int?, int?>)Music);
			script.AddFunction("sfx", (Action<int, int?, int?, int?>)Sfx);
		}

		public override void Update() {
			base.Update();
			Emulator.AudioBackend.Update();
		}

		public float[] RequestBuffer() {
			ClearBuffer();
			FillBuffer();
			CompressBuffer();

			Buffer.BlockCopy(audioBuffer, 0, externalAudioBuffer, 0, sizeof(float) * BufferSize);
			return externalAudioBuffer;
		}

		public void Sfx(int n, int? channel = -1, int? offset = 0, int? length = 32) {
			switch (n) {
				case -1:
					if (channel == -1) {
						StopAllChannelCount();
						break;
					}

					if (channel < 0 || channel >= ChannelCount)
						break;

					sfxChannels[channel.Value] = null;
					break;
				case -2:
					if (channel.Value < 0 || channel.Value >= ChannelCount)
						break;

					if (sfxChannels[channel.Value] != null) {
						sfxChannels[channel.Value].loop = false;
					}

					break;
				default:
					// If sound is already playing, stop it.
					int? index = FindSoundOnAChannel(n);

					if (index != null) {
						sfxChannels[index.Value] = null;
					}

					if (channel == -1) {
						channel = FindAvailableChannel();

						if (channel == null)
							break;
					}

					if (channel == -2) {
						break;
					}

					byte[] _sfxData = new byte[68];
					Buffer.BlockCopy(Emulator.Memory.ram, RamAddress.Sfx + 68 * n, _sfxData, 0, 68);

					var osc = new Oscillator(SampleRate);
					sfxChannels[channel.Value] = new Sfx(_sfxData, n, ref audioBuffer, ref osc, SampleRate);
					sfxChannels[channel.Value].CurrentNote = offset.Value;
					sfxChannels[channel.Value].LastIndex = offset.Value + length.Value - 1;
					sfxChannels[channel.Value].Start();
					break;
			}
		}

		public void Music(int n, int? fade_len = null, int? channel_mask = null) {
			// This is broken, so commented out for now
			//_musicPlayer.Start(n);
		}

		public void FillBuffer() {
			for (int i = 0; i < ChannelCount; i += 1) {
				var s = sfxChannels[i];

				if (s != null && !s.Update()) {
					sfxChannels[i] = null;
				}
			}

			_musicPlayer.Update();
		}

		public void ClearBuffer() {
			for (int i = 0; i < BufferSize; i++) {
				audioBuffer[i] = 0;
			}
		}

		public void CompressBuffer() {
			for (int i = 0; i < BufferSize; i++) {
				audioBuffer[i] = (float)Math.Tanh(audioBuffer[i]);
			}
		}

		private int? FindAvailableChannel() {
			for (int i = 0; i < ChannelCount; i += 1) {
				if (sfxChannels[i] == null) {
					return i;
				}
			}

			return null;
		}

		private int? FindSoundOnAChannel(int n) {
			for (int i = 0; i < ChannelCount; i += 1) {
				var s = sfxChannels[i];

				if (s != null && s.SfxIndex == n) {
					return i;
				}
			}

			return null;
		}

		private void StopChannel(int index) {
			sfxChannels[index] = null;
		}

		private void StopAllChannelCount() {
			for (int i = 0; i < ChannelCount; i += 1) {
				StopChannel(i);
			}
		}
	}
}