using BurningKnight.save;
using Lens.util.file;

namespace BurningKnight.state.save {
	public class SaveData : SaveNode {
		public SaveType SaveType;
		public SaveInspector Inspector;
		
		public void Load() {
			var stream = new FileReader(FullPath);
			SaveType = (SaveType) stream.ReadByte();

			switch (SaveType) {
				case SaveType.Game: 
					Inspector = new GameInspector();
					break;
				
				case SaveType.Global: 
					Inspector = new GlobalInspector();
					break;
				
				default:
					Inspector = new EntityInspector();
					break;
			}
			
			Inspector.Inspect(stream);
		}

		public override void Render() {
			Inspector.Render();			
		}
	}
}