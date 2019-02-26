using System;
using System.Collections.Generic;
using System.Threading;
using BurningKnight.entity.item;
using BurningKnight.entity.item.accessory.equippable;
using BurningKnight.entity.level.save;
using BurningKnight.ui;
using BurningKnight.util;
using Lens.assets;
using Lens.graphics;

namespace BurningKnight.game {
	public class Achievements {
		public const string REACH_DESERT = "REACH_DESERT_ACHIEVEMENT";
		public const string REACH_LIBRARY = "REACH_LIBRARY_ACHIEVEMENT";
		public const string REACH_FOREST = "REACH_DESERT_ACHIEVEMENT";
		public const string REACH_ICE = "REACH_LIBRARY_ACHIEVEMENT";
		public const string REACH_BLOOD = "REACH_DESERT_ACHIEVEMENT";
		public const string REACH_TECH = "REACH_LIBRARY_ACHIEVEMENT";
		public const string DIE = "DIE_ACHIEVEMENT";
		public const string BURN_TO_DEATH = "BURN_TO_DEATH_ACHIEVEMENT";
		public const string FIND_MIMIC = "FIND_MIMIC_ACHIEVEMENT";
		public const string COLLECT_300_GOLD = "COLLECT_300_GOLD_ACHIEVEMENT";
		public const string GET_8_HEART_CONTAINERS = "GET_8_HEART_CONTAINERS_ACHIEVEMENT";
		public const string KILL_BK = "KILL_BK_ACHIEVEMENT";
		public const string FIND_CRASH_BOOK = "FIND_CRASH_BOOK_ACHIEVEMENT";
		public const string FIND_SECRET_ROOM = "FIND_SECRET_ROOM_ACHIEVEMENT";
		public const string REALLY_KILL_BK = "RKILL_BK_ACHIEVEMENT";
		public const string KILL_DM = "KILL_DM_ACHIEVEMENT";
		public const string DONT_GET_HIT_IN_BOSS_FIGHT = "DONT_GET_HIT_IN_BOSS_FIGHT_ACHIEVEMENT";
		public const string SAVE_ALL = "SAVE_ALL_ACHIEVEMENT";
		public const string UNLOCK_BLACK_HEART = "UNLOCK_BLACK_HEART";
		public const string UNLOCK_MIMIC_TOTEM = "UNLOCK_MIMIC_TOTEM";
		public const string UNLOCK_MIMIC_SUMMONER = "UNLOCK_MIMIC_SUMMONER";
		public const string UNLOCK_SALE = "UNLOCK_SALE";
		public const string UNLOCK_MONEY_PRINTER = "UNLOCK_MONEY_PRINTER";
		public const string UNLOCK_CRASH_BOOK = "UNLOCK_CRASH_BOOK";
		public const string UNLOCK_BLOOD_CROWN = "UNLOCK_BLOOD_CROWN";
		public const string UNLOCK_DIAMOND_SWORD = "UNLOCK_DIAMOND_SWORD";
		public const string UNLOCK_DIAMOND = "UNLOCK_DIAMOND";
		public const string UNLOCK_HALO = "UNLOCK_HALO";
		public const string UNLOCK_WATER_BOLT = "UNLOCK_WATER_BOLT";
		public const string UNLOCK_WINGS = "UNLOCK_WINGS";
		public const string UNLOCK_DEW_VIAL = "UNLOCK_DEW_VIAL";
		public const string UNLOCK_LOOTPICK = "UNLOCK_LOOTPICK";
		public const string UNLOCK_SWORD_ORBITAL = "UNLOCK_SWORD_ORBITAL";
		public const string UNLOCK_AMMO_ORBITAL = "UNLOCK_AMMO_ORBITAL";
		public const string UNLOCK_SPECTACLES = "UNLOCK_SPECTACLES";
		public const string UNLOCK_MEATBOY = "UNLOCK_MEETBOY";
		public const string UNLOCK_MEATBOY_AXE = "UNLOCK_MEETBOY_AXE";
		public const string UNLOCK_GOLD_RING = "UNLOCK_GOLD_RING";
		public const string UNLOCK_BACKPACK = "UNLOCK_BACKPACK";
		public const string UNLOCK_ISAAC_HEAD = "UNLOCK_ISAAC_HEAD";
		public const string UNLOCK_DENDY = "UNLOCK_KOTLING_GUN";
		public const string UNLOCK_CLOCK_HEART = "UNLOCK_CLOCK_HEART";
		public const string UNLOCK_VVVVV = "UNLOCK_VVVVV";
		public const string UNLOCK_STOP_WATCH = "UNLOCK_STOP_WATCH";
		public const string UNLOCK_STAR = "UNLOCK_STAR";
		public const string UNLOCK_KOTLING_GUN = "UNLOCK_KOTLING_GUN";
		public const string SECRET_TRADER = "SECRET_TRADER";
		private static List<UiAchievement> ToShow = new List<UiAchievement>();
		private static Area Top = new Area(true);
		public static UiAchievement LastActive;

		public static bool Unlocked(string Id) {
			return Id == null || GlobalSave.IsTrue(Id);
		}

		private static void Save() {
			SaveManager.Save(SaveManager.Type.GLOBAL, false);
		}
		
		public static void Unlock(string Id) {
			if (Dungeon.Depth == -3) return;

			if (!Unlocked(Id)) {
				Log.Info(Id + " was unlocked!");
				GlobalSave[Id] = true;
				Thread thread = new Thread(Save);
				thread.Start();
				
				UiAchievement Achievement = new UiAchievement();
				
				if (Id.Contains("CLASS")) {
					Achievement.Text = Id.ToLower();
					Achievement.Extra = Id.ToLower() + "_desc";
					Achievement.Icon = Id.Equals("CLASS_MELEE")? Graphics.GetTexture("item-sword_a") : Graphics.GetTexture("item-gun_a");
					Achievement.Unlock = true;
				} else if (Id.Contains("ACHIEVEMENT")) {
					Achievement.Text = Id.ToLower();
					Achievement.Extra = Id.ToLower() + "_desc";
					Achievement.Icon = Graphics.GetTexture("achievement-" + (Id.ToLower().Replace("_achievement", "")));
				} else {
					string Reg = Id.Replace("UNLOCK_", "").Replace("SHOP_", "").ToLower();
					try {
						var Pair = ItemRegistry.Items.Get(Reg);
						
						if (Pair == null) {
							Log.Error("Failed to unlock item " + Reg);
							return;
						}
							
						var Item = (Item) Pair.Type.NewInstance();
						
						Achievement.Text = Item.GetName();
						Achievement.Icon = Item.GetSprite();
						Achievement.Unlock = true;
					} catch (Exception) {
						return;
					}
					
					ToShow.Add(Achievement);
				}
			}
		}
						
		public static void Clear() {
			Top.Destroy();
		}
			
		public static void Update(float Dt) {
			if ((LastActive == null || LastActive.Done) && ToShow.Count > 0) {
				LastActive = ToShow[0];
				ToShow.RemoveAt(0);
				Top.Add(LastActive);
			}
	
			Top.Update(Dt);
		}
		public static void Render() {
			Top.Render();
		}
	}
}