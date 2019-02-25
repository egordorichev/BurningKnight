using BurningKnight.core.entity.creature.npc;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class HatSelector : ItemHolder {
		public static List<HatSelector> All = new List<>();
		public bool Set;
		public static bool NullGot;
		private ItemRegistry.Pair Pair;
		private string Key;

		public override Void Init() {
			base.Init();
			All.Add(this);
		}

		public override Void Destroy() {
			if (this.GetItem() is NullItem) {
				NullGot = false;
			} 

			if (Pair != null) {
				Pair.Shown = false;
			} 

			All.Remove(this);
			base.Destroy();
		}

		public override Void Render() {
			base.Render();

			if ((!Set && this.GetItem() == null) || Upgrade.UpdateEvent) {
				foreach (Map.Entry<string, ItemRegistry.Pair> Pair in ItemRegistry.Items.EntrySet()) {
					if (!Pair.GetValue().Shown && Pair.GetValue().Pool == Upgrade.Type.DECOR && !Pair.GetKey().Equals(this.Key) && Pair.GetValue().Unlocked(Pair.GetKey())) {
						Pair.GetValue().Shown = true;
						this.Pair = Pair.GetValue();
						Key = Pair.GetKey();

						try {
							this.SetItem((Item) ItemRegistry.Items.Get(Pair.GetKey()).Type.NewInstance());
						} catch (InstantiationException | IllegalAccessException) {
							E.PrintStackTrace();
						}

						Set = true;

						break;
					} 
				}

				if (!Set && !NullGot && (Player.HatId != null && Player.HatId.Equals("gobbo_head"))) {
					Set = true;
					Key = "null";
					this.SetItem(new NullItem());
					NullGot = true;
				} 
			} 
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(Key);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Key = Reader.ReadString();
		}

		public override Void RenderShadow() {
			if (Set) {
				base.RenderShadow();
			} 
		}
	}
}
