using Microsoft.Xna.Framework.Content;

namespace Aseprite {
	public class AudioReader : ContentTypeReader<AudioFile> {
		protected override AudioFile Read(ContentReader input, AudioFile existingInstance) {
			if (existingInstance != null) {
				return existingInstance;
			}
			
			var stereo = input.ReadBoolean();
			var sampleRate = input.ReadInt32();
			var length = input.ReadInt32();
			var buffer = new float[length];

			for (var i = 0; i < buffer.Length; i++) {
				buffer[i] = input.ReadSingle();
			}

			return new AudioFile {
				Stereo = stereo,
				SampleRate = sampleRate,
				Buffer = buffer
			};
		}
	}
}