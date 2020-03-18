using System;
using System.Collections.Generic;
using System.Numerics;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save.statistics {
	/*
	 * todo:
	 * won
	 */
	public class RunStatistics : SaveableEntity {
		private const byte Version = 0;
		public bool Frozen;
		
		public float Time;
		public bool Won;
		public byte MaxDepth;
		public uint GameVersion;
		
		public List<string> Items = new List<string>();
		public List<string> Banned = new List<string>();

		public ushort CoinsObtained;
		public ushort BombsObtained;
		public ushort KeysObtained;
		
		public ushort HeartsCollected;
		public ushort MaxHealth;

		public uint DamageTaken;
		public uint DamageDealt;
		public uint MobsKilled;

		public uint RoomsExplored;
		public uint RoomsTotal;
		public uint SecretRoomsFound;
		public uint SecretRoomsTotal;

		public ushort Year;
		public ushort Day;

		public uint TilesWalked;
		public ushort PitsFallen;

		private float leftOver;
		
		public override void Load(FileReader stream) {
			var version = stream.ReadByte();

			if (version > Version) {
				Log.Error($"Unknown stastics version {version}");
				return;
			}

			Time = stream.ReadFloat();
			Won = stream.ReadBoolean();
			MaxDepth = stream.ReadByte();
			GameVersion = stream.ReadUInt32();

			var count = stream.ReadUInt16();
			Items.Clear();
			
			for (var i = 0; i < count; i++) {
				Items.Add(stream.ReadString());
			}
		
			count = stream.ReadUInt16();
			Banned.Clear();
			
			for (var i = 0; i < count; i++) {
				Banned.Add(stream.ReadString());
			}

			CoinsObtained = stream.ReadUInt16();
			BombsObtained = stream.ReadUInt16();
			KeysObtained = stream.ReadUInt16();
			
			HeartsCollected = stream.ReadUInt16();
			MaxHealth = stream.ReadUInt16();

			DamageTaken = stream.ReadUInt32();
			DamageDealt = stream.ReadUInt32();
			MobsKilled = stream.ReadUInt32();

			RoomsExplored = stream.ReadUInt32();
			RoomsTotal = stream.ReadUInt32();
			SecretRoomsFound = stream.ReadUInt32();
			SecretRoomsTotal = stream.ReadUInt32();

			Year = stream.ReadUInt16();
			Day = stream.ReadUInt16();

			TilesWalked = stream.ReadUInt32();
			PitsFallen = stream.ReadUInt16();
		}

		public override void Save(FileWriter stream) {
			GameVersion = BK.Version.Id;
			
			stream.WriteByte(Version);
			
			stream.WriteFloat(Time);
			stream.WriteBoolean(Won);
			stream.WriteByte(MaxDepth);
			stream.WriteUInt32(GameVersion);
		
			stream.WriteUInt16((ushort) Items.Count);

			foreach (var item in Items) {
				stream.WriteString(item);
			}
			
			stream.WriteUInt16((ushort) Banned.Count);

			foreach (var item in Banned) {
				stream.WriteString(item);
			}
			
			stream.WriteUInt16(CoinsObtained);
			stream.WriteUInt16(BombsObtained);
			stream.WriteUInt16(KeysObtained);
		
			stream.WriteUInt16(HeartsCollected);
			stream.WriteUInt16(MaxHealth);
			
			stream.WriteUInt32(DamageTaken);
			stream.WriteUInt32(DamageDealt);
			stream.WriteUInt32(MobsKilled);
			
			stream.WriteUInt32(RoomsExplored);
			stream.WriteUInt32(RoomsTotal);
			stream.WriteUInt32(SecretRoomsFound);
			stream.WriteUInt32(SecretRoomsTotal);
			
			stream.WriteUInt16(Year);
			stream.WriteUInt16(Day);
			
			stream.WriteUInt32(TilesWalked);
			stream.WriteUInt16(PitsFallen);
		}

		public override void Init() {
			base.Init();

			var data = DateTime.Now;

			Year = (ushort) data.Year;
			Day = (ushort) data.DayOfYear;
			
			Subscribe<RoomChangedEvent>();
			Subscribe<SecretRoomFoundEvent>();
			Subscribe<ItemAddedEvent>();
			Subscribe<PostHealthModifiedEvent>();
			Subscribe<DiedEvent>();
			Subscribe<ConsumableAddedEvent>();
			Subscribe<LostSupportEvent>();
			Subscribe<MaxHealthModifiedEvent>();
			Subscribe<NewLevelStartedEvent>();
		}

		public override void PostInit() {
			base.PostInit();

			Run.Statistics = this;
			AlwaysActive = true;
			
			MaxDepth = (byte) Math.Max(MaxDepth, Run.Depth);
			Time = Math.Max(Run.Time, Time);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Frozen) {
				return;
			}
			
			Time += dt;

			var player = LocalPlayer.Locate(Area);

			if (player == null) {
				return;
			}

			leftOver += (player.GetComponent<RectBodyComponent>().Velocity * dt).Length();

			var amount = (uint) Math.Floor(leftOver / 16f);
			leftOver -= amount * 16;
			TilesWalked += amount;
		}

		public override bool HandleEvent(Event e) {
			if (Frozen) {
				return false;
			}
			
			if (e is RoomChangedEvent rce) {
				if (!rce.WasDiscovered) {
					RoomsExplored++;
				}
			} else if (e is SecretRoomFoundEvent) {
				SecretRoomsFound++;
			} else if (e is ItemAddedEvent iae) {
				if (!iae.Item.Touched && !Items.Contains(iae.Item.Id)) {
					var t = iae.Item.Type;

					if (t != ItemType.Coin && t != ItemType.Heart && t != ItemType.Mana && t != ItemType.Key && t != ItemType.Bomb && t != ItemType.Battery) {
						Items.Add(iae.Item.Id);
					}
				}
			} else if (e is PostHealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					if (hme.From is Player) {
						DamageDealt += (uint) -hme.Amount;
					} else if (hme.Who is Player) {
						DamageTaken += (uint) -hme.Amount;
					}
				} else if (hme.Amount > 0 && hme.Who is Player) {
					HeartsCollected += (ushort) hme.Amount;
				}
			} else if (e is DiedEvent de) {
				if (de.Who is Player) {
					Won = false;
				} else if (de.Who is Mob) {
					MobsKilled++;
				}
			} else if (e is ConsumableAddedEvent cae) {
				switch (cae.Type) {
					case ItemType.Coin: {
						CoinsObtained += (ushort) cae.Amount;
						break;
					}

					case ItemType.Key: {
						KeysObtained += (ushort) cae.Amount;
						break;
					}
					
					case ItemType.Bomb: {
						BombsObtained += (ushort) cae.Amount;
						break;
					}
				}
			} else if (e is LostSupportEvent lse) {
				if (lse.Who is Player) {
					PitsFallen++;
				} 
			} else if (e is MaxHealthModifiedEvent mhme) {
				if (mhme.Who is Player) {
					MaxHealth = (ushort) (MaxHealth + mhme.Amount);
				}
			} else if (e is NewLevelStartedEvent) {
				foreach (var r in Area.Tagged[Tags.Room]) {
					if (((Room) r).Type == RoomType.Secret) {
						SecretRoomsTotal++;
					}

					RoomsTotal++;
				}
			}
			
			return false;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			RemoveTag(Tags.LevelSave);
			AddTag(Tags.PlayerSave);
		}

		public void RenderWindow() {
			if (!WindowManager.RunInfo) {
				return;
			}
			
			ImGui.SetWindowPos(Vector2.Zero, ImGuiCond.Once);
			
			if (!ImGui.Begin("Run Info")) {
				return;
			}
			
			ImGui.Text($"Time: {Math.Floor(Time / 3600f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time % 60f)}s");
			ImGui.Text($"Won: {Won}");
			ImGui.Text($"Max Depth: {MaxDepth}");
			ImGui.Text($"Game Version: {GameVersion}");
			Run.CalculateScore();
			ImGui.Text($"Score: {Run.Score}");
			ImGui.Separator();
			
			if (ImGui.TreeNode("Items")) {
				foreach (var item in Items) {
					ImGui.BulletText(item);
				}
				
				ImGui.TreePop();
			}
			
			if (ImGui.TreeNode("Banned items")) {
				foreach (var item in Banned) {
					ImGui.BulletText(item);
				}
				
				ImGui.TreePop();
			}

			ImGui.Separator();
			ImGui.Text($"Coins Collected: {CoinsObtained}");
			ImGui.Text($"Keys Collected: {KeysObtained}");
			ImGui.Text($"Bombs Collected: {BombsObtained}");
			ImGui.Text($"Hearts Collected: {HeartsCollected}");
			ImGui.Text($"Heart Containers Collected: {MaxHealth}");
			ImGui.Separator();
			
			
			ImGui.Text($"Damage Taken: {DamageTaken}");
			ImGui.Text($"Damage Dealt: {DamageDealt}");
			ImGui.Text($"Mobs Killed: {MobsKilled}");
			ImGui.Text($"Rooms Explored: {RoomsExplored} / {RoomsTotal}");
			ImGui.Text($"Secret Rooms Explored: {SecretRoomsFound} / {SecretRoomsTotal}");
			ImGui.Separator();

			ImGui.Text($"Date Started: {Day} {Year}");
			ImGui.Text($"Tiles Walked: {TilesWalked}");
			ImGui.Text($"Pits Fallen: {PitsFallen}");
			
			ImGui.Separator();
			ImGui.Text($"Luck: {Run.Luck}");
			ImGui.Text($"Scourge: {Run.Scourge}");
			
			if (ImGui.TreeNode("Scourges")) {
				foreach (var curse in Scourge.Defined) {
					var v = Scourge.IsEnabled(curse);

					if (ImGui.Checkbox(curse, ref v)) {
						if (v) {
							Scourge.Enable(curse);
						} else {
							Scourge.Disable(curse);
						}
					}
				}
				
				ImGui.TreePop();
			}
		}
	}
}