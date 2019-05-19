using BurningKnight.level;

namespace BurningKnight.ui.editor.command {
	public interface Command {
		void Do(Level level);
		void Undo(Level level);
	}
}