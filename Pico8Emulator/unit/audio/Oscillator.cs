using System;

namespace Pico8Emulator.unit.audio {
	public class Oscillator {
		/// <summary>
		/// Maps the waveform number in P8 to the actual waveform function.
		/// </summary>
		public Func<float, float>[] waveFuncMap;

		/// <summary>
		/// How many samples per second is retrieved.
		/// </summary>
		public float sampleRate;

		/// <summary>
		/// Tracks how much time has passed after each audio sample request.
		/// </summary>
		private float _time;

		/// <summary>
		/// Initializes a new instance of the <see cref="Oscillator"/> class.
		/// </summary>
		/// <param name="sampleRate">How many samples per second is retrieved.<see cref="float"/></param>
		public Oscillator(float sampleRate) {
			waveFuncMap = new Func<float, float>[] {
				Triangle,
				TiltedSaw,
				Sawtooth,
				Square,
				Pulse,
				Organ, // Organ
				Noise, // Noise
				Phaser // Phaser
			};

			this.sampleRate = sampleRate;
			_tscale = Util.NoteToFrequency(63) / sampleRate;
			_time = 0.0f;
		}

		/// <summary>
		/// Sine wave.
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Sine(float frequency) {
			_time += frequency / sampleRate;
			return (float)Math.Sin(_time * 2 * Math.PI);
		}

		/// <summary>
		/// Square wave.
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Square(float frequency) {
			return Sine(frequency) >= 0 ? 1.0f : -1.0f;
		}

		/// <summary>
		/// Pulse Wave 
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Pulse(float frequency) {
			_time += frequency / sampleRate;
			return ((_time) % 1 < 0.3125 ? 1 : -1) * 1.0f / 3.0f;
		}

		/// <summary>
		/// TiltedSaw Wave
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float TiltedSaw(float frequency) {
			_time += frequency / sampleRate;
			var t = (_time) % 1;
			return (((t < 0.875) ? (t * 16 / 7) : ((1 - t) * 16)) - 1) * 0.7f;
		}

		/// <summary>
		/// Sawtooth Wave
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Sawtooth(float frequency) {
			_time += frequency / sampleRate;
			return (float)(2 * (_time - Math.Floor(_time + 0.5)));
		}

		/// <summary>
		/// Triangle Wave
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Triangle(float frequency) {
			_time += frequency / sampleRate;
			return (Math.Abs(((_time) % 1) * 2 - 1) * 2.0f - 1.0f) * 0.7f;
		}

		/// <summary>
		/// Organ Wave
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Organ(float frequency) {
			_time += frequency / sampleRate;
			var x = _time * 4;
			return (float)((Math.Abs((x % 2) - 1) - 0.5f + (Math.Abs(((x * 0.5) % 2) - 1) - 0.5f) / 2.0f - 0.1f) * 0.7f);
		}

		/// <summary>
		/// Phaser Wave
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Phaser(float frequency) {
			_time += frequency / sampleRate;
			var x = _time * 2;
			return (Math.Abs((x % 2) - 1) - 0.5f + (Math.Abs(((x * 127 / 128) % 2) - 1) - 0.5f) / 2) - 1.0f / 4.0f;
		}

		private float _lastx = 0;
		private float _sample = 0;
		private float _tscale;
		private Random _random = new Random();

		/// <summary>
		/// White Noise Effect
		/// </summary>
		/// <param name="frequency">The frequency of the wave<see cref="float"/></param>
		/// <returns>The sample value<see cref="float"/></returns>
		public float Noise(float frequency) {
			_time += frequency / sampleRate;
			float scale = (_time - _lastx) / _tscale;
			float lsample = _sample;
			_sample = (lsample + scale * ((float)_random.NextDouble() * 2 - 1)) / (1.0f + scale);
			_lastx = _time;
			return Math.Min(Math.Max((lsample + _sample) * 4.0f / 3.0f * (1.75f - scale), -1), 1) * 0.7f;
		}
	}
}