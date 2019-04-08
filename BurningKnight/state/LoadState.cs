using System.Threading;
using BurningKnight.assets.lighting;
using BurningKnight.entity.level;
using BurningKnight.entity.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.entity;
using Lens.game;

namespace BurningKnight.state {
	public class LoadState : GameState {
		public string Path;
		private Area gameArea;
		
		public override void Init() {
			base.Init();
			
			Lights.Init();
			Physics.Init();
			gameArea = new Area();
			
			var thread = new Thread(() => {
				Tilesets.Load();
				
				if (Run.Id == -1) {
					SaveManager.Load(gameArea, SaveType.Game, Path);
				}

				SaveManager.Load(gameArea, SaveType.Level, Path);
				SaveManager.Load(gameArea, SaveType.Player, Path);
				
				Engine.Instance.SetState(new InGameState(gameArea));
			});

			thread.Priority = ThreadPriority.Lowest;
			thread.Start();
		}
	}
}