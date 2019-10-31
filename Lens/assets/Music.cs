using System;
using System.Reflection;
using Aseprite;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using Lens.util;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Music {
		private ISoundOut soundOut;
		private IWaveSource source;

		public float Volume;
		public bool Paused;
		public bool Repeat;

		public void Stop() {
			
		}
		
		public Music(string path) {
			/*source =
				CodecFactory.Instance.GetCodec(path)
					.ToSampleSource()
					.ToMono()
					.ToWaveSource();
			
			soundOut = new WasapiOut() {Latency = 100};
			soundOut.Initialize(source);
			soundOut.Play();*/
		}
	}
}