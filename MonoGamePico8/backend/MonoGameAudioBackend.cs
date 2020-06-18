using System;
using Microsoft.Xna.Framework.Audio;
using Pico8Emulator.backend;
using Pico8Emulator.unit.audio;

namespace MonoGamePico8.backend {
	public class MonoGameAudioBackend : AudioBackend {
		private DynamicSoundEffectInstance soundInstance;
		
		public MonoGameAudioBackend() {
			try {
				soundInstance = new DynamicSoundEffectInstance(AudioUnit.SampleRate, AudioChannels.Mono);
				GC.SuppressFinalize(soundInstance);
				soundInstance.Play();
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		public override void Destroy() {
			base.Destroy();
			soundInstance.Dispose();
		}

		public override void Update() {
			while (soundInstance.PendingBufferCount < 3) {
				var p8Buffer = Emulator.Audio.RequestBuffer();
				var samplesPerBuffer = p8Buffer.Length;
				var audioBuffer = new byte[samplesPerBuffer * 2];

				for (var i = 0; i < samplesPerBuffer; i += 1) {
					var floatSample = p8Buffer[i];

					var shortSample =
							(short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

					if (!BitConverter.IsLittleEndian) {
						audioBuffer[i * 2] = (byte) (shortSample >> 8);
						audioBuffer[i * 2 + 1] = (byte) shortSample;
					} else {
						audioBuffer[i * 2] = (byte) shortSample;
						audioBuffer[i * 2 + 1] = (byte) (shortSample >> 8);
					}
				}

				soundInstance.SubmitBuffer(audioBuffer);
			}
		}
	}
}