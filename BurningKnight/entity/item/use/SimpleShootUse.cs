using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		private static string[] manaDropNames = {
			"Where it lands",
			"Where it starts",
			"Where it starts, after death"
		};
		
		private float damage;
		private float speed;
		private float speedMax;
		private float range;
		private float scaleMin;
		private float scaleMax;
		private string slice;
		private float accuracy;
		private int count;
		private string prefab;
		private bool light;
		private bool disableBoost;
		private float knockback;
		private bool rect;
		private string sfx;
		private int sfx_number;
		private int manaUsage;
		protected bool wait;
		private bool toCursor;
		private string color;
		public bool ReloadSfx;
		private bool shells;
		private int manaDrop;
		private float angleAdd;
		
		public bool ProjectileDied = true;

		private ItemUse[] modifiers;

		public override void Use(Entity entity, Item item) {
			if (wait && !ProjectileDied) {
				return;
			}
			
			base.Use(entity, item);
		}

		private void PlaceMana(Area area, Vector2 where) {
			for (var j = 0; j < Math.Floor(manaUsage / 2f); j++) {
				Items.CreateAndAdd("bk:mana", area).Center = where;
			}

			if (manaUsage % 2 == 1) {
				Items.CreateAndAdd("bk:half_mana", area).Center = where;
			}
			
			AnimationUtil.Poof(where);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			modifiers = Items.ParseUses(settings["modifiers"]);

			if (modifiers != null) {
				foreach (var m in modifiers) {
					m.Item = Item;
				}
			}
			
			damage = settings["damage"].Number(1);
			speed = settings["speed"].Number(6);
			speedMax = settings["speedm"].Number(10);
			range = settings["range"].Number(0) * 0.7f;
			scaleMin = settings["scale"].Number(1);
			scaleMax = settings["scalem"].Number(1);
			slice = settings["texture"].String("rect");
			toCursor = settings["cursor"].Bool(false);
			sfx = settings["sfx"].String("item_gun_fire");
			sfx_number = settings["sfxn"].Int(0);
			ReloadSfx = settings["rsfx"].Bool(false);
			manaUsage = settings["mana"].Int(0);
			color = settings["color"].String("");
			angleAdd = settings["ang"].Number(0);

			if (manaUsage > 0) {
				manaDrop = settings["mdr"].Int(0);
			}

			if (slice == "default") {
				slice = "rect";
			}
			
			accuracy = settings["accuracy"].Number(0).ToRadians();
			count = settings["amount"].Int(1);
			prefab = settings["prefab"].String("");
			light = settings["light"].Bool(true);
			knockback = settings["knockback"].Number(1);
			rect = settings["rect"].Bool(false);
			wait = settings["wait"].Bool(false);
			disableBoost = settings["dsb"].Bool(false);
			shells = settings["shells"].Bool(true);

			SpawnProjectile = (entity, item) => {
				if (manaUsage > 0) {
					var mana = entity.GetComponent<ManaComponent>();

					if (mana.Mana < manaUsage) {
						AnimationUtil.ActionFailed();
						return;
					}
					
					mana.ModifyMana(-manaUsage);
				}
				
				var bad = entity is Creature c && !c.IsFriendly();
				var sl = slice;
				
				if (bad) {
					sl = "small";
				}

				entity.TryGetComponent<StatsComponent>(out var stats);

				if (sfx == "item_gun_fire") {
					entity.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed(sfx, 2, 0.5f, sz: 0.2f);
				} else {
					if (sfx_number == 0) {
						entity.GetComponent<AudioEmitterComponent>().EmitRandomized(sfx, 0.5f, sz: 0.25f);
					} else {
						entity.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed(sfx, sfx_number, 0.5f, sz: 0.25f);
					}
				}

				var aim = entity.GetComponent<AimComponent>();
				var from = toCursor ? entity.Center : aim.Center;
				var am = toCursor ? Input.Mouse.GamePosition : aim.RealAim;
				var a = MathUtils.Angle(am.X - from.X, am.Y - from.Y);
				var pr = prefab.Length == 0 ? null : ProjectileRegistry.Get(prefab);
				var ac = accuracy;

				if (stats != null) {
					ac /= stats.Accuracy;
				}
				
				var cnt = count;
				var accurate = false;

				if (entity is Player pl) {
					var e = new PlayerShootEvent {
						Player = (Player) entity
					};

					entity.HandleEvent(e);

					cnt += e.Times - 1;
					accurate = e.Accurate;

					if (sl == "rect") {
						sl = pl.ProjectileTexture;
					}
				}

				for (var i = 0; i < cnt; i++) {
					var angle = a;
					
					if (accurate) {
						angle += (i - (int) (cnt * 0.5f)) * 0.2f + Rnd.Float(-ac / 2f, ac / 2f);
					} else if (cnt == 1 || i > 0) {
						angle += Rnd.Float(-ac / 2f, ac / 2f);
					}

					angle += angleAdd * (float) Math.PI * 2;

					var antiAngle = angle - (float) Math.PI;
					var projectile = Projectile.Make(entity, sl, angle, Rnd.Float(speed, speedMax), !rect, 0, null, Rnd.Float(scaleMin, scaleMax), damage * (item.Scourged ? 1.5f : 1f), Item);
					projectile.Boost = !disableBoost;
					projectile.Item = item;

					Camera.Instance.Push(antiAngle, 4f);
					entity.GetComponent<RectBodyComponent>()?.KnockbackFrom(antiAngle, 0.4f * knockback);

					var clr = bad ? Projectile.RedLight : ProjectileColor.Yellow;
					
					if (!string.IsNullOrEmpty(color) && ProjectileColor.Colors.TryGetValue(color, out clr)) {
						projectile.Color = clr;
					}
					
					if (light) {
						projectile.AddLight(32f, clr);
					}

					projectile.FlashTimer = 0.05f;

					if (range > 0.01f) {
						if (Math.Abs(projectile.Range - (-1)) < 0.1f) {
							projectile.Range = range / speed;
						} else {
							projectile.Range += range / speed;
						}
					}
					
					projectile.Center = from;

					if (modifiers != null) {
						foreach (var m in modifiers) {
							if (m is ModifyProjectilesUse mpu) {
								mpu.ModifyProjectile(projectile);
							}
						}
					}
					
					pr?.Invoke(projectile);

					if (wait && i == 0) {
						ProjectileDied = false;
						projectile.OnDeath += (prj, e, t) => ProjectileDied = true;
					}

					if (manaUsage > 0) {
						if (manaDrop == 0) {
							projectile.OnDeath += (prj, e, t) => {
								PlaceMana(entity.Area, prj.Center);
							};	
						} else if (manaDrop == 1) {
							PlaceMana(entity.Area, entity.Center);
						} else {
							var where = entity.Center;
							
							projectile.OnDeath += (prj, e, t) => {
								PlaceMana(entity.Area, where);
							};	
						}
					}
				}

				if (shells) {
					Timer.Add(() => {
						var p = new ShellParticle(new Particle(Controllers.Destroy, new TexturedParticleRenderer {
							Region = CommonAse.Particles.GetSlice("shell")
						}));

						p.Position = entity.Center;
						p.Y += Rnd.Float(-4, 10);

						entity.Area.Add(p);

						var f = (entity.CenterX > Input.Mouse.GamePosition.X ? 1 : -1);

						p.Particle.Velocity =
							new Vector2(f * Rnd.Float(40, 60), 0) + entity.GetAnyComponent<BodyComponent>().Velocity;

						p.Particle.Angle = 0;
						p.Particle.Zv = Rnd.Float(1.5f, 2.5f);
						p.Particle.AngleVelocity = f * Rnd.Float(40, 70);

						p.AddShadow();
					}, 0.2f);
				}
			};
		}

		private class ShellParticle : ParticleEntity {
			private bool played;
			
			public ShellParticle(Particle particle) : base(particle) {
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!played && Particle.Z <= 0) {
					played = true;
					AddComponent(new AudioEmitterComponent());
					GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("projectile_shell", 3);
				}
			}
		}

		public static void RenderDebug(JsonValue root) {
			if (root.InputInt("Mana Usage", "mana", 0) > 0) {
				var b = root["mdr"].Int(0);

				if (ImGui.Combo("Place Mana", ref b, manaDropNames, manaDropNames.Length)) {
					root["mdr"] = b;
				}
			}

			if (ImGui.TreeNode("Stats")) {
				root.Checkbox("To Cursor", "cursor", false);
				root.InputFloat("Damage", "damage");
				root.InputInt("Projectile Count", "amount");
				root.InputText("Sound", "sfx", "item_gun_fire");
				root.InputInt("Sound Prefix Number", "sfxn", 0);
				root.Checkbox("Reload Sound", "rsfx", false);
				root.Checkbox("Drop Shells", "shells", true);
				
				var c = root.InputText("Color", "color");

				if (!string.IsNullOrEmpty(c) && !ProjectileColor.Colors.ContainsKey(c)) {
					ImGui.BulletText("Unknown color");
				}

				ImGui.Separator();

				root.InputFloat("Min Speed", "speed", 10);
				root.InputFloat("Max Speed", "speedm", 10);
				root.Checkbox("Disable Boost", "dsb", false);

				ImGui.Separator();
				
				root.InputFloat("Min Scale", "scale");
				root.InputFloat("Max Scale", "scalem");
				
				ImGui.Separator();

				var range = (float) root["range"].Number(0);

				if (ImGui.InputFloat("Range", ref range)) {
					root["range"] = range;
				}

				var knockback = (float) root["knockback"].Number(1);

				if (ImGui.InputFloat("Knockback", ref knockback)) {
					root["knockback"] = knockback;
				}

				root.InputFloat("Additional Angle", "ang", 0);
				var accuracy = (float) root["accuracy"].Number(0);

				if (ImGui.InputFloat("Accuracy", ref accuracy)) {
					root["accuracy"] = accuracy;
				}

				var light = root["light"].Bool(true);

				if (ImGui.Checkbox("Light", ref light)) {
					root["light"] = light;
				}

				var rect = root["rect"].Bool(false);

				if (ImGui.Checkbox("Rect body", ref rect)) {
					root["rect"] = rect;
				}

				var wait = root["wait"].Bool(false);

				if (ImGui.Checkbox("Wait for projectile death", ref wait)) {
					root["wait"] = wait;
				}

				var prefab = root["prefab"].String("");

				if (ImGui.InputText("Prefab", ref prefab, 128)) {
					root["prefab"] = prefab;
				}

				if (prefab.Length > 0 && ProjectileRegistry.Get(prefab) == null) {
					ImGui.BulletText("Unknown prefab");
				}

				var slice = root["texture"].String("rect");

				if (slice == "default") {
					slice = "rect";
				}
				
				var region = CommonAse.Projectiles.GetSlice(slice, false);

				if (region != null) {
					ImGui.Image(ImGuiHelper.ProjectilesTexture, new Num.Vector2(region.Width * 3, region.Height * 3),
						new Num.Vector2(region.X / region.Texture.Width, region.Y / region.Texture.Height),
						new Num.Vector2((region.X + region.Width) / region.Texture.Width,
							(region.Y + region.Height) / region.Texture.Height));
				}

				if (ImGui.InputText("Texture", ref slice, 128)) {
					root["texture"] = slice;
				}

				ImGui.TreePop();
			}

			if (ImGui.TreeNode("Modifiers")) {
				if (!root["modifiers"].IsJsonArray) {
					root["modifiers"] = new JsonArray();
				}

				ItemEditor.DisplayUse(root, root["modifiers"], "bk:ModifyProjectiles");
				ImGui.TreePop();
			}
		}
	}
}