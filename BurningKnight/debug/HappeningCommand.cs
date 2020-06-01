using System;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch.happening;
using Lens;
using Lens.util;

namespace BurningKnight.debug {
	public class HappeningCommand : ConsoleCommand {
		public HappeningCommand() {
    	Name = "HappeningCommand";
    	ShortName = "hp";
    }
    
    public override void Run(Console Console, string[] Args) {
    	if (Args.Length == 1) {
	      var h = HappeningRegistry.Get(Args[0]);

	      if (h == null) {
		      return;
	      }

	      try {
					h.Happen(LocalPlayer.Locate(Engine.Instance.State.Area));
	      } catch (Exception e) {
		      Log.Error(e);
	      }
      }
    }
	}
}