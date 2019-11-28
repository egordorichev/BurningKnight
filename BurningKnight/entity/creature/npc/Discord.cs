using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class Discord : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 12;
			
			AddComponent(new AnimationComponent("discord"));
			
			// todo: connect discordIntegration CurrentPlayer here
			AddComponent(new CloseDialogComponent("discord_0"));
			GetComponent<DialogComponent>().Dialog.Voice = 2;
		}
	}
}