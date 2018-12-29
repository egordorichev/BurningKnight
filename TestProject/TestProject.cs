using Lens;
using Lens.Pattern.Observer;
using Lens.State;
using Lens.Util;
using TestProject.Pattern.Observers.Sfx;

namespace TestProject {
	public class TestProject : Engine {
		public TestProject(GameState state, string title, int width, int height, bool fullscreen) : base(state, title, width, height, fullscreen) {
			
		}

		protected override void Initialize() {
			base.Initialize();
			Subjects.Audio.Add(new SfxObserver());
		}
	}
}