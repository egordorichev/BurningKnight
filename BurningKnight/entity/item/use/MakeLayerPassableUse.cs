using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.projectile;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeLayerPassableUse : ItemUse {
		private bool forProjectiles;
		private bool forPlayer;

		private bool chasms;
		private bool props;
		private bool walls;
		
		public override void Use(Entity entity, Item item) {
			if (forProjectiles) {
				if (props) {
					Projectile.ShouldIgnoreProps = true;
				}

				if (walls) {
					Projectile.ShouldIgnoreWalls = true;
				}
			}
			
			if (forPlayer) {
				if (props) {
					Player.ShouldIgnoreProps = true;
				}

				if (chasms) {
					((Creature) entity).Flying = true;
				}

				if (walls) {
					Player.ShouldIgnoreWalls = true;
				}
			}	
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			forProjectiles = settings["fp"].Bool(false);
			forPlayer = settings["fpl"].Bool(true);
			chasms = settings["ic"].Bool(false);
			props = settings["ip"].Bool(false);
			walls = settings["iw"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			var v = root["fp"].Bool(false);

			if (ImGui.Checkbox("For projectiles", ref v)) {
				root["fp"] = v;
			}
			
			v = root["fpp"].Bool(false);

			if (ImGui.Checkbox("For player", ref v)) {
				root["fpp"] = v;
			}
			
			ImGui.Separator();
			
			v = root["ic"].Bool(false);

			if (ImGui.Checkbox("Ignore chasms", ref v)) {
				root["ic"] = v;
			}
			
			v = root["ip"].Bool(false);

			if (ImGui.Checkbox("Ignore props", ref v)) {
				root["ip"] = v;
			}
			
			v = root["iw"].Bool(false);

			if (ImGui.Checkbox("Ignore walls", ref v)) {
				root["iw"] = v;
			}
		}
	}
}