using Microsoft.Xna.Framework.Content.Pipeline;


namespace Aseprite {
	[ContentImporter(".aseprite", ".ase", DefaultProcessor = "AsepriteProcessor", DisplayName = "Aseprite Importer")]
	public class AsepriteImporter : ContentImporter<AsepriteFile> {
		public override AsepriteFile Import(string filename, ContentImporterContext context) {
			return new AsepriteFile(filename, context.Logger);
		}
	}
}