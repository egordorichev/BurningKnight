using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;
using SharpDX.Direct2D1.Effects;

namespace BurningKnight.entity.item.use {
	public class ModifyArcUse : ItemUse {
		public float Damage;
		public float Chance;
		public bool ToAny;
		public bool EventCreated = true;
		public string BuffToApply;
		public bool InfiniteBuff;
		public float BuffDuration;
		public bool RandomEffect;
		public float EffectChangeSpeed;
		public bool Mine;
		
		private float lastEffectTime;
		private int lastEffect = -1;

		private static string[] effects = {
			BurningBuff.Id, FrozenBuff.Id, PoisonBuff.Id, CharmedBuff.Id, SlowBuff.Id, BrokenArmorBuff.Id
		};

		public override void Use(Entity entity, Item item) {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is MeleeArc.CreatedEvent pce) {
				var a = Item == pce.By;
				
				if (ToAny || (EventCreated && a)) {
					ModifyProjectile(pce.Arc);
				}
			}

			return base.HandleEvent(e);
		}

		public void ModifyProjectile(MeleeArc arc) {
			if (Rnd.Float() > Chance + Run.Luck * 0.2f) {
				return;
			}

			arc.Mines = Mine;
			arc.Damage *= Damage;

			if (RandomEffect) {
				if (lastEffect == -1 || (EffectChangeSpeed > 0 && Engine.Time - lastEffectTime >= EffectChangeSpeed)) {
					lastEffect = Rnd.Int(effects.Length);
					lastEffectTime = Engine.Time;
				}

				ApplyBuff(arc, effects[lastEffect]);
				
				return;
			}
			
			if (BuffToApply != null) {
				ApplyBuff(arc, BuffToApply);
			}
		}

		private void ApplyBuff(MeleeArc arc, string buff) {
			if (buff != null) {
				if (!BuffRegistry.All.TryGetValue(buff, out var info)) {
					Log.Error($"Unknown buff {buff}");

					return;
				}

				arc.Color = info.Effect.GetColor();

				arc.OnHurt += (p, e) => {
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
			Damage = settings["damage"].Number(1);
			ToAny = settings["any"].Bool(false);
			Mine = settings["mine"].Bool(false);

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
			root.InputFloat("Damage Modifier", "damage");
			root.Checkbox("Make it mine", "mine", false);

			ImGui.Separator();

			if (root.Checkbox("Random Effect", "rne", false)) {
				root.InputFloat("Effect Change Speed", "ecs", 3f);
			} else {
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