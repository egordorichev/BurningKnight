using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Aseprite {
	[ContentImporter(".ogg", DefaultProcessor = "AudioProcessor", DisplayName = "Audio Importer")]
	public class AudioImporter : ContentImporter<AudioFile> {
		public static AudioFile Load(string filename) {
			using (var vorbis = new NVorbis.VorbisReader(filename)) {
				var channels = vorbis.Channels;
				var sampleRate = vorbis.SampleRate;
				var channelSize = sampleRate * vorbis.TotalTime.TotalSeconds;
				var bufferSize = (int) Math.Ceiling(channels * channelSize);
				var readBuffer = new float[bufferSize];

				vorbis.ReadSamples(readBuffer, 0, bufferSize);

				return new AudioFile {
						SampleRate = sampleRate,
						Stereo = channels == 2,
						Buffer = readBuffer
				};
			}
		}
		
		public override AudioFile Import(string filename, ContentImporterContext context) {
			return Load(filename);
		}
	}
}