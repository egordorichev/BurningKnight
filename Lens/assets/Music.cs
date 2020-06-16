using System;
using Lens.util;
using Microsoft.Xna.Framework.Audio;
using NAudio.Vorbis;

namespace Lens.assets {
	public class Music {
		private float[] buffer;
		private int channels;
		private int sampleRate;
		private int bufferLength;
		
		public bool Repeat = true;
		public bool Paused = false;
		public float Volume = 1;

		public readonly string Id;
		public int BufferLength => bufferLength;

		public Music(string musicFile) {
			Id = musicFile;
			
			Log.Info($"Started loading {musicFile}");
			LoadMusic(musicFile);
			Log.Info($"Ended loading {musicFile}");
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
			using (var reader = new VorbisWaveReader(musicFile)) {
				channels = reader.WaveFormat.Channels;
				sampleRate = reader.WaveFormat.SampleRate;
				
				Log.Info($"Sample rate is {sampleRate}, channels {channels}");
				
				var channelSize = sampleRate * reader.TotalTime.TotalSeconds;
				var bufferSize = (int) Math.Ceiling(channels * channelSize);
				
				buffer = new float[bufferSize];
				reader.Read(buffer, 0, bufferSize);
				bufferLength = bufferSize / channels;
				
				if (Audio.SoundEffectInstance == null) {
					Audio.SoundEffectInstance = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Stereo);
					Audio.SoundEffectInstance.BufferNeeded += Audio.SubmitBuffer;
					Audio.SoundEffectInstance.Play();
				}
			}
		}

		public float GetSample(uint position, int channel) {
			if (position >= bufferLength) {
				if (!Repeat) {
					return 0;
				}

				position = (uint) (position % bufferLength);
			}
			
			return Paused ? 0 : buffer[(position * channels + channel)] * Volume;
		}
	}
}