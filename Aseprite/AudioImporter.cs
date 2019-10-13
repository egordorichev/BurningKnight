using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Aseprite {
	[ContentImporter(".ogg", DefaultProcessor = "AudioProcessor", DisplayName = "Audio Importer")]
	public class AudioImporter : ContentImporter<AudioFile> {
		public override AudioFile Import(string filename, ContentImporterContext context) {
			using (var vorbis = new NVorbis.VorbisReader(filename)) {
				var channels = vorbis.Channels;
				var sampleRate = vorbis.SampleRate;
				var channelSize = sampleRate * vorbis.TotalTime.TotalSeconds;
				var bufferSize = (int) Math.Ceiling(channels * channelSize);
				var readBuffer = new float[bufferSize];

				vorbis.ReadSamples(readBuffer, 0, bufferSize);

				var buffer = new byte[bufferSize * channels * 2];

				for (int i = 0; i < channelSize; i++) {
					for (int c = 0; c < channels; c++) {
						float floatSample = readBuffer[i * channels + c];
						
						short shortSample = (short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

						int index = i * channels * 2 + c * 2;

						// Store the 16 bit sample as two consecutive 8 bit values in the buffer with regard to endian-ness
						if (!BitConverter.IsLittleEndian) {
							buffer[index] = (byte) (shortSample >> 8);
							buffer[index + 1] = (byte) shortSample;
						}else {
							buffer[index] = (byte) shortSample;
							buffer[index + 1] = (byte) (shortSample >> 8);
						}
					}
				}

				return new AudioFile {
					SampleRate = sampleRate,
					Stereo = channels == 2,
					Buffer = buffer
				};
			}
		}
	}
}