using System.Threading;
using BurningKnight.entity.level;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.entity;
using Lens.game;

namespace BurningKnight.state {
	public class LoadState : GameState {
		private Area gameArea;
		
		public override void Init() {
			base.Init();

			Physics.Init();
			gameArea = new Area();
			
			var thread = new Thread(() => {
				Tilesets.Load();
				
				if (Run.Id == -1) {
					SaveManager.Load(gameArea, SaveManager.SaveType.Game);
				}

				SaveManager.Load(gameArea, SaveManager.SaveType.Level);
				SaveManager.Load(gameArea, SaveManager.SaveType.Player);
				
				Engine.Instance.SetState(new InGameState(gameArea));
			});

			thread.Priority = ThreadPriority.Lowest;
			thread.Start();
		}
	}
}