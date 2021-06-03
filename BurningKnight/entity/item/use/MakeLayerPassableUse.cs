using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using BurningKnight.util;
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
		private bool mobs;
		private bool stones;
		private bool projectiles;
		private bool breakProjectiles;

		public override void Use(Entity entity, Item item) {
			if (forPlayer) {
				if (props) {
					CollisionFilterComponent.Add(entity, (o, e) => e is Prop ? CollisionResult.Disable : CollisionResult.Default);
				}

				if (chasms) {
					((Creature) entity).Flying = true;
				}

				if (walls) {
					CollisionFilterComponent.Add(entity, (o, en) => en is Level || en is ProjectileLevelBody || en is HalfProjectileLevel || en is HalfWall ? CollisionResult.Disable : CollisionResult.Default);
				}
				
				if (stones) {
					CollisionFilterComponent.Add(entity, (o, en) => en is HalfWall || en is HalfProjectileLevel ? CollisionResult.Disable : CollisionResult.Default);
				}
			}	
		}

		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				if (forProjectiles) {
					if (props) {
						CollisionFilterComponent.Add(pce.Projectile, (o, en) => en is Prop ? CollisionResult.Disable : CollisionResult.Default);
					}

					if (walls) {
						pce.Projectile.AddFlags(ProjectileFlags.FlyOverStones);
						CollisionFilterComponent.Add(pce.Projectile, (o, en) => (en is Level || en is ProjectileLevelBody || en is Door) ? CollisionResult.Disable : CollisionResult.Default);
					}

					if (mobs) {
						CollisionFilterComponent.Add(pce.Projectile, (o, en) => en is Creature ? CollisionResult.Disable : CollisionResult.Default);
					}
				
					if (stones) {
						CollisionFilterComponent.Add(pce.Projectile, (o, en) => en is HalfWall || en is HalfProjectileLevel ? CollisionResult.Disable : CollisionResult.Default);
					}
					
					if (projectiles) {
						CollisionFilterComponent.Add(pce.Projectile, (o, en) => en is Projectile ? CollisionResult.Enable : CollisionResult.Default);
					}

					if (breakProjectiles) {
						pce.Projectile.AddFlags(ProjectileFlags.BreakOtherProjectiles);
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			forProjectiles = settings["fp"].Bool(false);
			forPlayer = settings["fpl"].Bool(false);
			chasms = settings["ic"].Bool(false);
			props = settings["ip"].Bool(false);
			walls = settings["iw"].Bool(false);
			mobs = settings["im"].Bool(false);
			stones = settings["st"].Bool(false);
			projectiles = settings["p"].Bool(false);
			breakProjectiles = settings["bp"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			var v = root["fp"].Bool(false);

			if (ImGui.Checkbox("For projectiles", ref v)) {
				root["fp"] = v;
			}
			
			v = root["fpl"].Bool(false);

			if (ImGui.Checkbox("For player", ref v)) {
				root["fpl"] = v;
			}


			ImGui.Separator();

			root.Checkbox("Projectiles", "p", false);
			
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
			
			v = root["im"].Bool(false);

			if (ImGui.Checkbox("Ignore mobs", ref v)) {
				root["im"] = v;
			}

			root.Checkbox("Ignore stones", "st", false);
			root.Checkbox("Break Projectiles", "bp", false);
		}
	}
}