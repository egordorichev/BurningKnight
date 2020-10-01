using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.ui.inventory;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.input;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Tombstone : Prop {
		public string Item;
		public bool DisableDialog;
		public byte Index;
		public bool HasPlayer;
		public bool WasGamepad;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			
			AddComponent(new RoomComponent());
			AddComponent(new DialogComponent());

			if (!DisableDialog) {
				AddComponent(new CloseDialogComponent("tomb_0") {
					CanTalk = e => Item != null
				});
			}

			AddComponent(new RectBodyComponent(0, 2, 12, 14, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			
			AddComponent(new InteractableSliceComponent("props", "tombstone"));

			AddComponent(new ShadowComponent());
			AddComponent(new HealthComponent {
				RenderInvt = !DisableDialog,
				InitMaxHealth = 3
			});
			
			AddComponent(new LightComponent(this, 64, new Color(0.7f, 0.6f, 0.3f, 1f)));
			
			Subscribe<RoomChangedEvent>();
			GetComponent<DialogComponent>().Dialog.Voice = 30;
		}

		public static Player CreatePlayer(Area area, byte index, bool gamepad, Vector2 where) {
			InGameState.Multiplayer = true;
			
			Player p;
			var input = area.Add(p = new LocalPlayer()).GetComponent<InputComponent>();
			
			input.Index = index;
			input.KeyboardEnabled = !gamepad;
			input.GamepadEnabled = gamepad;
			
			p.BottomCenter = where;
			
			var cursor = new Cursor {
				Player = p
			};

			var u = new UiInventory(p, true);

			u.ForceUpdate = true;
			
			((InGameState) Engine.Instance.State).Ui.Add(u);
			((InGameState) Engine.Instance.State).TopUi.Add(cursor);

			p.GetComponent<CursorComponent>().Cursor = cursor;
			
			AnimationUtil.Poof(where, 1);
			Camera.Instance.Shake(16);
			
			return p;
		}

		public bool Revive(Entity e) {
			Item = null;
			UpdateSprite();
			var p = CreatePlayer(Area, Index, WasGamepad, BottomCenter + new Vector2(0, 2));
			Index = 255;

			var h1 = e.GetComponent<HealthComponent>();
			var hr1 = e.GetComponent<HeartsComponent>();
			var h2 = p.GetComponent<HealthComponent>();
			var hr2 = p.GetComponent<HeartsComponent>();
			
			p.GetComponent<ActiveWeaponComponent>().Set(Items.CreateAndAdd(Items.Generate(ItemPool.StartingWeapon), Area));

			h1.InvincibilityTimer = 0;
			h1.Unhittable = false;
			h2.InvincibilityTimer = 0;
			h2.Unhittable = false;
			
			if (h1.Health > 0) {
				var half = (int) Math.Max(1, Math.Floor(h1.Health / 2f));

				h1.SetHealth(half, e, true, DamageType.Custom);
				h2.SetHealth(half, e, true, DamageType.Custom);
			} else if (hr1.ShieldHalfs > 0) {
				var half = (int) Math.Max(1, Math.Floor(hr1.ShieldHalfs / 2f));
				hr1.ModifyShields(-(hr1.ShieldHalfs - half), e);
				hr2.ModifyShields(-(hr2.ShieldHalfs - half), e);
			} else {
				var half = (int) Math.Max(1, Math.Floor(hr1.Bombs / 2f));
				hr2.BombsMax = hr1.BombsMax;

				hr1.ModifyBombs(-(hr1.Bombs - half), e);
				hr2.ModifyBombs(-(hr2.Bombs - half), e);
			}

			if (p.GetComponent<InputComponent>().Index == 0) {
				var minIndex = 1024;
				Player pl = null;

				foreach (var pr in Area.Tagged[Tags.Player]) {
					var i = pr.GetComponent<InputComponent>().Index;

					if (p != pr && i < minIndex) {
						minIndex = i;
						pl = (Player) pr;
					}
				}

				if (pl != null) {
					var c = pl.ForceGetComponent<ConsumablesComponent>();
					c.Entity = p;
					pl.Components.Remove(typeof(ConsumablesComponent));
					p.Components[typeof(ConsumablesComponent)] = c;
					pl.AddComponent(new ConsumablesComponent());
				}
			}

			return true;
		}
		
		public override void PostInit() {
			base.PostInit();

			if (HasPlayer) {
				AddComponent(new InteractableComponent(Revive) {
					CanInteract = e => Index != 255,
					OnStart = entity => {
						if (entity is LocalPlayer) {
							Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get("revive")));
						}
					},
				});
			} else if (!DisableDialog) {
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = e => Item != null
				});
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent d) {
				Interact(d.From);
				return true;
			} else if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					// Daddy? What did they do with you?!?!
					// rce.Who.GetComponent<DialogComponent>().StartAndClose("player_0", 3f);
				}
			}
			
			return base.HandleEvent(e);
		}

		private bool Interact(Entity entity) {
			if (Item == null) {
				return true;
			}
		
			var i = Items.CreateAndAdd(Item, entity.Area);

			if (i != null) {
				i.CenterX = CenterX;
				i.Y = Bottom;
				i.Scourged = true;
			}
			
			Item = null;
			UpdateSprite();
			Run.AddScourge(true);

			GetComponent<DialogComponent>().Close();
			
			AnimationUtil.Poof(Center);
			Camera.Instance.Shake(16);
			
			// fixme: spawn ghosts, dialog should not appear when player is ded
			
			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			Item = stream.ReadString();
			UpdateSprite();

			WasGamepad = stream.ReadBoolean();

			if (stream.ReadBoolean()) {
				HasPlayer = true;
				Index = stream.ReadByte();
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteString(Item);
			stream.WriteBoolean(WasGamepad);
			stream.WriteBoolean(HasPlayer);
			
			if (HasPlayer) {
				stream.WriteByte(Index);
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			var has = Item != null;

			if (ImGui.Checkbox("Has item", ref has)) {
				Item = has ? "" : null;
				UpdateSprite();
			}

			if (has) {
				ImGui.InputText("Item##itm", ref Item, 128);
			}
		}

		private void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Set("props", Item == null ? "broken_tombstone" : "tombstone");
		}
	}
}