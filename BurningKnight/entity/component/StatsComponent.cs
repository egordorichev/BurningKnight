using ImGuiNET;
using Lens.entity.component;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class StatsComponent : SaveableComponent {
		private float speed = 1;
		private float damage = 1;
		private float fireRate = 1;
		private float accuracy = 1;
		private float range = 1;

		public float DMChance;
		public float GrannyChance;
		public bool SawDeal;
		public bool TookDeal;

		public float Speed {
			get => speed;
			set => speed = MathUtils.Clamp(0.1f, 3f, value);
		}
		
		public float Damage {
			get => damage;
			set => damage = MathUtils.Clamp(0.1f, 100f, value);
		}
		
		public float FireRate {
			get => fireRate;
			set => fireRate = MathUtils.Clamp(0.1f, 3f, value);
		}
		
		public float Accuracy {
			get => accuracy;
			set => accuracy = MathUtils.Clamp(0.1f, 10f, value);
		}
		
		public float Range {
			get => range;
			set => range = MathUtils.Clamp(0.1f, 3f, value);
		}

		public override void RenderDebug() {
			base.RenderDebug();

			ImGui.InputFloat("Speed", ref speed);
			ImGui.InputFloat("Damage", ref damage);
			ImGui.InputFloat("Fire Rate", ref fireRate);
			ImGui.InputFloat("Accuracy", ref accuracy);
			ImGui.InputFloat("Range", ref range);
			
			ImGui.Separator();
			
			ImGui.InputFloat("DM Chance", ref DMChance);
			ImGui.InputFloat("Granny Chance", ref GrannyChance);

			ImGui.Checkbox("Saw Deal", ref SawDeal);
			ImGui.Checkbox("Took Deal", ref TookDeal);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteFloat(DMChance);
			stream.WriteFloat(GrannyChance);
			stream.WriteBoolean(SawDeal);
			stream.WriteBoolean(TookDeal);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			DMChance = stream.ReadFloat();
			GrannyChance = stream.ReadFloat();
			SawDeal = stream.ReadBoolean();
			TookDeal = stream.ReadBoolean();
		}
	}
}