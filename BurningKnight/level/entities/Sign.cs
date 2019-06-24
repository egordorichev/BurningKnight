using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.util.file;

namespace BurningKnight.level.entities {
	public class Sign : Prop {
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			var d = GetComponent<CloseDialogComponent>(); 
			stream.WriteString(d.Variants.Length == 0 ? "" : d.Variants[0]);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			SetMessage(stream.ReadString());
		}

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
			var m = d.Variants.Length == 0 ? "" : d.Variants[0];

			if (ImGui.InputText("Message", ref m, 128)) {
				SetMessage(m);
			}
		}
	}
}