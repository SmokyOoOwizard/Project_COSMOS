using System.Collections.Generic;
using COSMOS.Core;
using System;

namespace COSMOS.Common.Inventory
{
    public class InventorySlot : EventDispatcher
    {
        public Item Item { get; protected set; }

        public virtual bool Lock { get; protected set; }

        protected readonly List<Func<Item, bool>> rules = new List<Func<Item, bool>>();

        public virtual bool PlaceItem(Item item)
        {
            if (item == null || Item != null || Lock)
            {
                return false;
            }
            Item = item;
            dispatchEvent(Inventory.Events.AddItem, new ItemEventData(item));
            return true;
        }
        public virtual bool ReplaceItem(Item oldItem, Item newItem)
        {
            if (oldItem == null || Item != oldItem || Lock)
            {
                return false;
            }
            Item = newItem;
            dispatchEvent(Inventory.Events.ReplaceItem, new ReplaceItemEventData(oldItem, newItem));
            return true;
        }
        public virtual bool SwapItem(InventorySlot slot)
        {
            Item newItem = slot.Item;
            if (Lock || slot.Lock)
            {
                return false;
            }
            if (newItem == Item)
            {
                return true;
            }
            if (Item == null && newItem != null)
            {
                if (CheckItem(newItem))
                {
                    Item = newItem;
                    slot.Item = null;
                    dispatchEvent(Inventory.Events.ReplaceItem, new ReplaceItemEventData(Item, newItem));
                    return true;
                }
            }
            else if (Item != null && slot.Item == null)
            {
                if (slot.CheckItem(Item))
                {
                    slot.Item = Item;
                    Item = null;
                    dispatchEvent(Inventory.Events.ReplaceItem, new ReplaceItemEventData(Item, newItem));
                    return true;
                }
            }
            else if (CheckItem(newItem) && slot.CheckItem(Item))
            {
                slot.Item = Item;
                Item = newItem;
                dispatchEvent(Inventory.Events.ReplaceItem, new ReplaceItemEventData(Item, newItem));
                return true;
            }
            return false;
        }
        public virtual bool RemoveItem(Item item)
        {
            if (item == null || Item != item || Lock)
            {
                return false;
            }
            Item = null;
            dispatchEvent(Inventory.Events.RemoveItem, new ItemEventData(item));
            return true;
        }

        public void AddRule(Func<Item, bool> rule)
        {
            if (rule != null)
            {
                rules.Remove(rule);
                rules.Add(rule);
            }
        }
        public bool RemoveRule(Func<Item, bool> rule)
        {
            if (rule != null)
            {
                return rules.Remove(rule);
            }
            return false;
        }
        public virtual bool CheckItem(Item item)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                if (!rules[i].Invoke(item))
                {
                    return false;
                }
            }
            return true;
        }

    }
}