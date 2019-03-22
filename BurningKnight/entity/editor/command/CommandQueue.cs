using System.Collections.Generic;
using BurningKnight.entity.level;

namespace BurningKnight.entity.editor.command {
	public class CommandQueue {
		private Stack<Command> commands = new Stack<Command>();
		public Editor Editor;
		
		public void Do(Command command) {
			commands.Push(command);
			command.Do(Editor.Level);
		}

		public void Undo() {
			if (commands.Count == 0) {
				return;
			}

			var command = commands.Pop();
			command.Undo(Editor.Level);
		}
	}
}