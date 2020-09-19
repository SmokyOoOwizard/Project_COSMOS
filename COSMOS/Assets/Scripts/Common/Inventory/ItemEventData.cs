using COSMOS.Core;

namespace COSMOS.Common.Inventory
{
    public struct ItemEventData : IEventData
    {
        public Item Item;

        public ItemEventData(Item item)
        {
            this.Item = item;
        }
    }
}