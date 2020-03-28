using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.util.file;

namespace BurningKnight.level.entities {
	public class Sign : Prop {
		public string Region = "sign";
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			var d = GetComponent<CloseDialogComponent>(); 
			stream.WriteString(d.Variants.Length == 0 ? "" : d.Variants[0]);
			stream.WriteString(Region);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			SetMessage(stream.ReadString());
			Region = stream.ReadString() ?? "sign";
		}

		public void SetMessage(string m) {
			GetComponent<CloseDialogComponent>().Variants = new [] { m };
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new DialogComponent());
			AddComponent(new CloseDialogComponent());
			AddComponent(new ShadowComponent());
			
			GetComponent<DialogComponent>().Dialog.Voice = 30;
		}

		public override void PostInit() {
			base.PostInit();
			UpdateSprite();
		}

		private void UpdateSprite() {
			if (HasComponent<SliceComponent>()) {
				RemoveComponent<SliceComponent>();
			}

			var c = new SliceComponent("props", Region);
			AddComponent(c);
			
			Width = c.Sprite.Width;
			Height = c.Sprite.Height;
		}
		
		public override void RenderImDebug() {
			var d = GetComponent<CloseDialogComponent>();
			var m = d.Variants.Length == 0 ? "" : d.Variants[0];

			if (ImGui.InputText("Message", ref m, 128)) {
				SetMessage(m);
			}
			
			if (ImGui.InputText("Sprite", ref Region, 128)) {
				UpdateSprite();
			}
		}
	}
}