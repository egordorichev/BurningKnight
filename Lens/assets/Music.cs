using System;
using System.Reflection;
using Aseprite;
using Lens.util;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Music {
		public const int BufferSize = 2048;
		
		public DynamicSoundEffectInstance SoundInstance;
		public byte[] Buffer;
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
		
		public Music(AudioFile source) {
			Buffer = source.Buffer;
			SampleRate = source.SampleRate;
			Stereo = source.Stereo;

			SoundInstance = new DynamicSoundEffectInstance(SampleRate, Stereo ? AudioChannels.Stereo : AudioChannels.Mono);
			SoundInstance.Play();
			dynamicBuffer = new byte[BufferSize];
		}
		
		private static object GetInstanceField(Type type, object instance, string fieldName) {
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			                                     | BindingFlags.Static);
			return field?.GetValue(instance);
		}

		public void Update() {
			while (SoundInstance.PendingBufferCount < 3) {
				var bufferLength = Buffer.Length;
				
				for (var i = 0; i < BufferSize; i++) {
					dynamicBuffer[i] = Buffer[position];
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