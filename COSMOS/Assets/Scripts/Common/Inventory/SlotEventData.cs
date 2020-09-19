using COSMOS.Core;

namespace COSMOS.Common.Inventory
{
    public struct SlotEventData : IEventData
    {
        public InventorySlot Slot;
        public SlotEventData(InventorySlot slot)
        {
            this.Slot = slot;
        }
    }
}