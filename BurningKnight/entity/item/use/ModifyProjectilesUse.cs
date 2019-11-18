using System;
using BurningKnight.assets.items;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class ModifyProjectilesUse : ItemUse {
		public float Scale;
		public float Damage;
		public float Chance;
		public bool EventCreated = true;
		public string BuffToApply;
		public bool InfiniteBuff;
		public float BuffDuration;

		public override void Use(Entity entity, Item item) {
			
		}

		public override bool HandleEvent(Event e) {
			if (EventCreated && Item.Owner.GetComponent<ActiveWeaponComponent>().Item == Item && e is ProjectileCreatedEvent pce) {
				ModifyProjectile(pce.Projectile);
			}
			
			return base.HandleEvent(e);
		}

		public void ModifyProjectile(Projectile projectile) {
			if (Rnd.Float() > Chance) {
				return;
			}
			
			projectile.Scale *= Scale;
			projectile.Damage = (int) Math.Round(projectile.Damage * Damage);

			if (BuffToApply != null) {
				if (!BuffRegistry.All.TryGetValue(BuffToApply, out var info)) {
					Log.Error($"Unknown buff {BuffToApply}");
					return;
				}

				projectile.Effect = info.Effect;
				
				projectile.OnHurt += (p, e) => {
					if (e.TryGetComponent<BuffsComponent>(out var buffs)) {
						var b = BuffRegistry.Create(BuffToApply);

						if (InfiniteBuff) {
							b.Infinite = true;
						} else {
							b.TimeLeft = b.Duration = BuffDuration;
						}

						buffs.Add(b);
					}
				};
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);			
			
			Chance = settings["chance"].Number(1);
			Scale = settings["amount"].Number(1);
			Damage = settings["damage"].Number(1);
			
			BuffToApply = settings["buff"].String(null);

			if (BuffToApply != null) {
				InfiniteBuff = settings["infinite_buff"].Bool(false);
				BuffDuration = settings["buff_time"].Number(10);
			}
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Chance", "chance");
			root.InputFloat("Scale Modifier", "amount");
			root.InputFloat("Damage Modifier", "damage");
			
			if (ImGui.TreeNode("Buff")) {
				if (!BuffRegistry.All.ContainsKey(root.InputText("Buff", "buff", "bk:frozen"))) {
					ImGui.BulletText("Unknown buff!");
				}

				if (!root.Checkbox("Infinite", "infinite_buff", false)) {
					root.InputFloat("Buff Duration", "buff_time");
				}

				ImGui.TreePop();
			}
		}
	}
}