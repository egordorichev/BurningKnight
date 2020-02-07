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
using Lens;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class ModifyProjectilesUse : ItemUse {
		public float Scale;
		public float Damage;
		public float Chance;
		public bool ToAny;
		public bool EventCreated = true;
		public string BuffToApply;
		public bool InfiniteBuff;
		public float BuffDuration;
		public bool Explosive;
		public bool RandomEffect;
		public float EffectChangeSpeed;

		private float lastEffectTime;
		private int lastEffect = -1;

		private static string[] effects = {
			BurningBuff.Id, FrozenBuff.Id, PoisonBuff.Id
		};

		public override void Use(Entity entity, Item item) {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				var a = Item == pce.Item;
				
				if (ToAny || (EventCreated && a)) {
					ModifyProjectile(pce.Projectile);
				}
			}

			return base.HandleEvent(e);
		}

		public void ModifyProjectile(Projectile projectile) {
			if (Rnd.Float() > Chance + Run.Luck * 0.2f) {
				return;
			}
			
			projectile.Scale *= Scale;
			projectile.Damage = (int) Math.Round(projectile.Damage * Damage);

			if (RandomEffect) {
				if (lastEffect == -1 || (EffectChangeSpeed > 0 && Engine.Time - lastEffectTime >= EffectChangeSpeed)) {
					lastEffect = Rnd.Int(effects.Length + 1);
					lastEffectTime = Engine.Time;
				}

				var e = lastEffect == effects.Length;
				ApplyBuff(projectile, e ? null : effects[lastEffect], e);
				
				return;
			}
			
			if (BuffToApply != null || Explosive) {
				ApplyBuff(projectile, BuffToApply, Explosive);
			}
		}

		private void ApplyBuff(Projectile projectile, string buff, bool explosive) {
			if (explosive) {
				projectile.Damage = 0;
				projectile.Color = ProjectileColor.Brown;
				projectile.OnDeath += (p, t) => {
					ExplosionMaker.Make(p, 32, false, damage: 8, scale: 0.5f);
				};
			}

			if (buff != null) {
				if (!BuffRegistry.All.TryGetValue(buff, out var info)) {
					Log.Error($"Unknown buff {buff}");

					return;
				}

				projectile.Color = info.Effect.GetColor();

				projectile.OnHurt += (p, e) => {
					if (e.TryGetComponent<BuffsComponent>(out var buffs)) {
						var b = BuffRegistry.Create(buff);

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
			ToAny = settings["any"].Bool(false);
			Explosive = settings["explosive"].Bool(false);
			
			BuffToApply = settings["buff"].String(null);

			if (BuffToApply != null) {
				InfiniteBuff = settings["infinite_buff"].Bool(false);
				BuffDuration = settings["buff_time"].Number(10);
			} else {
				BuffDuration = 10;
			}

			RandomEffect = settings["rne"].Bool(false);

			if (RandomEffect) {
				EffectChangeSpeed = settings["ecs"].Number(3f);
			}
		}
		
		public static void RenderDebug(JsonValue root) {
			root.Checkbox("From Any Source", "any", false);
			
			root.InputFloat("Chance", "chance");
			root.InputFloat("Scale Modifier", "amount");
			root.InputFloat("Damage Modifier", "damage");
			
			ImGui.Separator();

			if (root.Checkbox("Random Effect", "rne", false)) {
				root.InputFloat("Effect Change Speed", "ecs", 3f);
			} else {
				root.Checkbox("Make Explosive", "explosive", false);

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
}