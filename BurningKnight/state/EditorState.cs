using BurningKnight.entity.editor;
using BurningKnight.entity.level;
using Lens.game;

namespace BurningKnight.state {
	public class EditorState : GameState {
		private Editor editor;
		
		public override void Init() {
			base.Init();
			
			Tilesets.Load();
			Area.Add(editor = new Editor());
		}
	}
}