using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.level.biome;
using BurningKnight.save;
using BurningKnight.state;
using ImGuiNET;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.plant {
	public class Plant : Prop {
		private static string[] variants = {
			"plant_a", "plant_b", "plant_c", "plant_d", "plant_e", "plant_f",
			"plant_i", "plant_j", "plant_k", "plant_l", "plant_m", "plant_n", "plant_o" 
		};

		public byte Variant;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (GraphicsComponent == null) {
				var s = variants[Variant % variants.Length];
				var g = Variant == 255 ? new PlantGraphicsComponent("props", "cabbadge")
					: new PlantGraphicsComponent($"{Run.Level.Biome.Id}_biome", $"{s}{(Variant >= variants.Length ? "s" : "")}");
				
				AddComponent(g);
				g.Flipped = Rnd.Chance();

				Width = g.Sprite.Width;
				Height = g.Sprite.Height;

				var caves = Run.Level.Biome is CaveBiome;

				if (Variant != 255 && (caves || Run.Depth != 0 || (s != "plant_k" && s != "plant_m"))) {
					if (caves) {
						AddComponent(new LightComponent(this, Rnd.Int(16, 32), new Color(0.4f, 0.1f, 0.4f, 1f)));
					} else if (s == "plant_m") {
						AddComponent(new LightComponent(this, 24, new Color(0.4f, 0.4f, 1f, 1f)));
					} else if (s == "plant_k") {
						AddComponent(new LightComponent(this, 24, new Color(1f, 0.4f, 0.4f, 1f)));
					}
				}
			
				AddComponent(new ShadowComponent());
			}
		}

		public override void Init() {
			base.Init();
			Variant = (byte) Rnd.Int(variants.Length * 2);
		}
		
		public override void Load(FileReader stream) {
			base.Load(stream);
			Variant = stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte(Variant);
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			var v = (int) Variant;

			if (ImGui.InputInt("Id", ref v)) {
				Variant = (byte) v;
				RemoveComponent<PlantGraphicsComponent>();
			}
		}
	}
}