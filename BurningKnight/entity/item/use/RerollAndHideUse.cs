namespace BurningKnight.entity.item.use {
	public class RerollAndHideUse : RerollItemsUse {
		protected override void ProcessItem(Item item) {
			base.ProcessItem(item);
			item.Unknown = true;
		}
	}
}