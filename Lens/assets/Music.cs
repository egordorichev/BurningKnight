using System;
using System.Reflection;
using Aseprite;
using Lens.util;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Music {
		public DynamicSoundEffectInstance SoundInstance;
		public byte[] Buffer;
		public int SampleRate;
		public bool Stereo;
		public float Volume = 1f;
		public float Position;
		public bool Paused;
		public bool Repeat = true;

		public Music(AudioFile source) {
			Buffer = source.Buffer;
			SampleRate = source.SampleRate;
			Stereo = source.Stereo;
		}
		
		private static object GetInstanceField(Type type, object instance, string fieldName) {
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			                                     | BindingFlags.Static);
			return field?.GetValue(instance);
		}

		public void Update() {
			
		}

		public void PlayFromStart() {
			Paused = false;
			Position = 0;
		}

		public void Stop() {
			Paused = true;
			Position = 0;
		}
	}
}