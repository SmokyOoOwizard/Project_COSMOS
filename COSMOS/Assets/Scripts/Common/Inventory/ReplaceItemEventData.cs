using COSMOS.Core;

namespace COSMOS.Common.Inventory
{
    public struct ReplaceItemEventData : IEventData
    {
        public Item OldItem;
        public Item NewItem;
        public ReplaceItemEventData(Item oldItem, Item newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }
}