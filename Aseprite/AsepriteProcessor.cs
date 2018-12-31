using Microsoft.Xna.Framework.Content.Pipeline;

namespace Aseprite {
	[ContentProcessor(DisplayName = "Aseprite Processor")]
	public class AsepriteProcessor : ContentProcessor<AsepriteFile, ProcessedAseprite> {
		public override ProcessedAseprite Process(AsepriteFile input, ContentProcessorContext context) {
			return new ProcessedAseprite {
				Aseprite = input,
				Log = context.Logger
			};
		}
	}

	public class ProcessedAseprite {
		public AsepriteFile Aseprite;
		public ContentBuildLogger Log;
	}
}