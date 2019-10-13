using System;
using System.Reflection;
using Lens.util;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Music {
		public const int SampleRate = 44100;
		
		public DynamicSoundEffectInstance SoundInstance;
		public byte[] Buffer;
		public float Volume = 1f;
		public float Position;
		public bool Paused;
		public bool Repeat = true;

		public Music(SoundEffect source) {
			var data = GetInstanceField(typeof(SoundEffect), source, "SoundBuffer");
			
			if (data == null) {
				Log.Error("Failed to grab data from sound effect");
				return;
			}

			Log.Info("Grabbed sound data from the effect!");
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