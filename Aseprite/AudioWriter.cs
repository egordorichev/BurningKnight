using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Aseprite {
	[ContentTypeWriter]
	public class AudioWriter : ContentTypeWriter<AudioFile> {
		public override string GetRuntimeType(TargetPlatform targetPlatform) {
			Type type = typeof(AsepriteReader);
			return type.Namespace + ".AudioReader, " + type.AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform) {
			Type type = typeof(AsepriteReader);
			return type.Namespace + ".AudioReader, " + type.Assembly.FullName;
		}

		protected override void Write(ContentWriter output, AudioFile value) {
			output.Write(value.Stereo);
			output.Write(value.SampleRate);
			output.Write(value.Buffer.Length);
			output.Write(value.Buffer);
		}
	}
}