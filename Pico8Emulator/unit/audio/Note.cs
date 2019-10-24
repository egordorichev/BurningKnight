using System;

namespace Pico8Emulator.unit.audio {
	public class Note {
		private float[] _audioBuffer;
		private float _duration;
		private float _fadeIn;
		private float _fadeOut;
		private float _timePassed;
		private int _sampleRate;
		public bool isCustom;
		public float targetVolume;
		public byte waveform;
		public byte pitch;

		private bool _vibrato;
		private float _volume;
		private int _pitchFrom;
		private Oscillator _oscillator;

		public Note(ref float[] audioBuffer, int sampleRate, ref Oscillator oscillator, float duration, byte volume,
			byte waveform, byte pitch, int pitchFrom = -1, float fadeIn = 1, float fadeOut = 1, bool vibrato = false) {
			_audioBuffer = audioBuffer;

			_duration = duration;
			_sampleRate = sampleRate;

			_fadeIn = fadeIn * duration / 100.0f;
			_fadeOut = fadeOut * duration / 100.0f;

			_timePassed = 0.0f;
			_volume = 1;

			isCustom = waveform > 7;
			targetVolume = volume / 7.0f;
			this.waveform = waveform;
			this.pitch = pitch;

			_pitchFrom = pitchFrom == -1 ? pitch : pitchFrom;

			_oscillator = oscillator;
			_vibrato = vibrato;
		}

		public int Process(int bufferOffset = 0, bool writeToBuffer = true) {
			for (int i = bufferOffset; i < AudioUnit.BufferSize; i++) {
				if (writeToBuffer) {
					if (_timePassed < _fadeIn) {
						_volume = Util.Lerp(0, targetVolume, _timePassed / _fadeIn);
					}
					else if (_timePassed > _duration - _fadeOut) {
						_volume = Util.Lerp(targetVolume, 0, (_timePassed - (_duration - _fadeOut)) / _fadeOut);
					}
					else {
						_volume = targetVolume;
					}

					if (_timePassed >= _duration) {
						return i;
					}

					float freq;

					if (_vibrato) {
						freq = Util.Lerp(Util.NoteToFrequency(pitch), Util.NoteToFrequency(pitch + 0.5f),
							(float)Math.Sin(_timePassed * 2 * Math.PI * 8));
					}
					else {
						freq = Util.Lerp(Util.NoteToFrequency(_pitchFrom), Util.NoteToFrequency(pitch), _timePassed / _duration);
					}

					float sample = _oscillator.waveFuncMap[waveform](freq);
					_audioBuffer[i] += sample * _volume;
				}

				_timePassed += 1.0f / _sampleRate;
			}

			return AudioUnit.BufferSize;
		}
	}
}