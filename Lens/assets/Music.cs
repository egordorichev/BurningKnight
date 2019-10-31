using System;
using Lens.util;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;

namespace Lens.assets {
	public class Music {
		private const int BufferDuration = 100;
		
		private int position;
		private int count;
		private byte[] byteArray;
		
		public bool Repeat = true;

		public bool Paused {
			get => Audio.SoundEffectInstance.State != SoundState.Playing;

			set {
				if (value) {
					Audio.SoundEffectInstance.Pause();					 
				} else {
					Audio.SoundEffectInstance.Play();
				}
			}
		}

		public float Volume {
			get => Audio.SoundEffectInstance.Volume;
			set => Audio.SoundEffectInstance.Volume = value;
		}

		public Music(string musicFile) {			
			LoadMusic(musicFile);
		}

		public void Play() {
			Audio.SoundEffectInstance.Play();
		}

		public void Stop() {
			Audio.SoundEffectInstance.Stop();
		}

		private static int AlignTo8Bytes(int unalignedBytes) {
			var result = unalignedBytes + 4;
			result -= (result % 8);
			return result;
		}

		private void LoadMusic(string musicFile) {
			using (var reader = new WaveFileReader(musicFile)) {
				byteArray = new byte[reader.Length];
				var read = reader.Read(byteArray, 0, byteArray.Length);

				Log.Debug($"Differenece: {byteArray.Length - read}, sample: {reader.WaveFormat.SampleRate}");
			
				
				if (Audio.SoundEffectInstance == null) {
					Audio.SoundEffectInstance = new DynamicSoundEffectInstance(reader.WaveFormat.SampleRate, AudioChannels.Stereo);
				}

				count = AlignTo8Bytes(Audio.SoundEffectInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(BufferDuration)) + 4);
				Audio.SoundEffectInstance.BufferNeeded += Audio.UpdateBuffer;
				Audio.SoundEffectInstance.Play();
			}
		}

		public void UpdateBuffer() {
			Audio.SoundEffectInstance.SubmitBuffer(byteArray, position, count / 2);
			Audio.SoundEffectInstance.SubmitBuffer(byteArray, position + count / 2, count / 2);

			position += count;

			if (position + count > byteArray.Length) {
				if (!Repeat) {
					Audio.SoundEffectInstance.Stop();
				}
				
				position = 0;
			}
		}
	}
}