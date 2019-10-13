using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Aseprite {
	[ContentImporter(".ogg", DefaultProcessor = "AudioProcessor", DisplayName = "Audio Importer")]
	public class AudioImporter : ContentImporter<AudioFile> {
		public override AudioFile Import(string filename, ContentImporterContext context) {
			using (var vorbis = new NVorbis.VorbisReader(filename)) {
				var channels = vorbis.Channels;
				var sampleRate = vorbis.SampleRate;
				var bufferSize = channels * sampleRate / 5;
				var readBuffer = new float[bufferSize];

				var cnt = 0;
				var position = 0;
				
				while ((cnt = vorbis.ReadSamples(readBuffer, 0, readBuffer.Length)) > 0) {
					position += cnt;
				}
				
				context.Logger.LogMessage($"Got to position {position}");

				return new AudioFile {
					SampleRate = sampleRate,
					Stereo = channels == 2,
					Buffer = new byte[2]
				};
			}
		}
	}
}