using System.Collections.Generic;

namespace COSMOS.Common.Inventory
{
    public interface IInventory
    {
        IEnumerator<InventorySlot> GetSlots();
        IEnumerator<Item> GetItems();
        bool AddItem(Item item);
        bool ReplaceItem(Item oldItem, Item newItem);
        bool RemoveItem(Item item);
    }
}