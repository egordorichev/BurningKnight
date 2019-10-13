using System;
using System.Reflection;
using Aseprite;
using Lens.util;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Music {
		public const int BufferSize = 2048 * 4;
		
		public DynamicSoundEffectInstance SoundInstance;
		public float[] Buffer;
		public int SampleRate;
		public bool Stereo;
		public float Volume = 1f;

		public bool Paused {
			get => SoundInstance.State == SoundState.Playing;

			set {
				if (value) {
					//SoundInstance.Play();
				} else {
					//SoundInstance.Pause();
				}
			}
		}
		
		public bool Repeat = true;

		private byte[] dynamicBuffer;
		private int position;
		private byte channels;
		
		public Music(AudioFile source) {
			Buffer = source.Buffer;
			SampleRate = source.SampleRate;
			Stereo = source.Stereo;
			channels = (byte) (Stereo ? 2 : 1);

			SoundInstance = new DynamicSoundEffectInstance(SampleRate, Stereo ? AudioChannels.Stereo : AudioChannels.Mono);
			SoundInstance.Play();
			dynamicBuffer = new byte[BufferSize * 2];
		}
		
		private static object GetInstanceField(Type type, object instance, string fieldName) {
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			                                     | BindingFlags.Static);
			return field?.GetValue(instance);
		}

		public void Update() {
			while (SoundInstance.PendingBufferCount < 3) {
				var bufferLength = Buffer.Length / channels;
				var dynamicBufferLength = BufferSize / channels;

				for (var i = 0; i < dynamicBufferLength; i++) {
					for (var c = 0; c < channels; c++) {
						var floatSample = Buffer[position * channels + c] * Volume;
						var shortSample = (short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);
						var index = (i * channels + c) * 2;

						if (!BitConverter.IsLittleEndian) {
							dynamicBuffer[index] = (byte) (shortSample >> 8);
							dynamicBuffer[index + 1] = (byte) shortSample;
						} else {
							dynamicBuffer[index] = (byte) shortSample;
							dynamicBuffer[index + 1] = (byte) (shortSample >> 8);
						}
					}
					
					position = (position + 1) % bufferLength;
				}

				SoundInstance.SubmitBuffer(dynamicBuffer);
			}
		}

		public void PlayFromStart() {
			Paused = false;
			position = 0;
		}

		public void Stop() {
			Paused = true;
			position = 0;
		}
	}
}