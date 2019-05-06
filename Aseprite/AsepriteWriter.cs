using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Aseprite {
	[ContentTypeWriter]
	class AsepriteWriter : ContentTypeWriter<ProcessedAseprite> {
		protected override void Write(ContentWriter output, ProcessedAseprite value) {			
			AsepriteFile aseprite = value.Aseprite;

			output.Write(aseprite.Width);
			output.Write(aseprite.Height);

			// Layers
			output.Write(aseprite.Layers.Count);
			
			foreach (var layer in aseprite.Layers) {
				output.Write(layer.Name);
				output.Write(layer.Opacity);
				output.Write((int) layer.BlendMode);
				output.Write(layer.Flag.HasFlag(AsepriteLayer.Flags.Visible));
			}

			// Frames
			output.Write(aseprite.Frames.Count);
			var f = 0;
			
			foreach (var frame in aseprite.Frames) {
				output.Write(frame.Duration);
				output.Write(frame.Cels.Count);
				
				for (int i = 0; i < frame.Cels.Count; i++) {
					var cel = frame.Cels[i];
					output.Write(aseprite.Layers.IndexOf(cel.Layer));
					output.Write(cel.X);
					output.Write(cel.Y);
					output.Write(cel.Width);
					output.Write(cel.Height);

					for (int celY = 0; celY < cel.Height; celY++) {
						for (int celX = 0; celX < cel.Width; celX++) {
							int ind = celX + celY * cel.Width;
							output.Write(cel.Pixels[ind]);
						}
					}
				}

				f++;
			}

			// Animations
			output.Write(aseprite.Tags.Count);
			
			foreach (var animation in aseprite.Tags) {
				output.Write(animation.Name);
				output.Write(animation.From);
				output.Write(animation.To);
				output.Write((byte) animation.LoopDirection);
			}
		}

		public override string GetRuntimeType(TargetPlatform targetPlatform) {
			Type type = typeof(AsepriteReader);
			return type.Namespace + ".AsepriteReader, " + type.AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform) {
			Type type = typeof(AsepriteReader);
			return type.Namespace + ".AsepriteReader, " + type.Assembly.FullName;
		}
	}
}