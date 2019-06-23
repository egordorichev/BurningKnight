using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using ImGuiNET;

namespace BurningKnight.level.entities {
	public class Sign : Prop {
		public void SetMessage(string m) {
			GetComponent<CloseDialogComponent>().Variants = new [] { m };
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 16;
			Height = 14;
			
			AddComponent(new SliceComponent("props", "sign"));
			AddComponent(new DialogComponent());
			AddComponent(new CloseDialogComponent());
		}
		
		public override void RenderImDebug() {
			var d = GetComponent<CloseDialogComponent>();
			var m = d.Variants[0];

			if (ImGui.InputText("Message", ref m, 128)) {
				SetMessage(m);
			}
		}
	}
}