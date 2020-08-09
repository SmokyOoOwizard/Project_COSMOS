using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using COSMOS.Core;
using System;

namespace COSMOS.Common.Inventory
{
    public class Inventory : EventDispatcher<Inventory.Events>, IInventory
    {
        public enum Events
        {
            AddItem,
            ReplaceItem,
            RemoveItem,
            AddSlot,
            RemoveSlot
        }

        protected readonly List<Func<Item, bool>> Rules = new List<Func<Item, bool>>();

        protected readonly List<InventorySlot> Slots = new List<InventorySlot>();
        protected readonly List<InventorySlot> FreeSlots = new List<InventorySlot>();

        public void AddRule(Func<Item, bool> rule)
        {
            if (rule != null)
            {
                Rules.Remove(rule);
                Rules.Add(rule);
            }
        }
        public bool RemoveRule(Func<Item, bool> rule)
        {
            if (rule != null)
            {
                return Rules.Remove(rule);
            }
            return false;
        }
        public virtual bool CheckItem(Item item)
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                if (!Rules[i].Invoke(item))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool AddItem(Item item)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                if (!slot.Lock)
                {
                    if (slot.Item == null)
                    {
                        if (slot.PlaceItem(item))
                        {
                            FreeSlots.Remove(slot);
                            Dispatch(Events.AddItem, item);
                            return true;
                        }
                    }
                    else if (slot.PlaceItem(item))
                    {
                        Dispatch(Events.AddItem, item);
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual bool ReplaceItem(Item oldItem, Item newItem)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                if (!slot.Lock)
                {
                    if (slot.ReplaceItem(oldItem, newItem))
                    {
                        if (slot.Item == null)
                        {
                            FreeSlots.Add(slot);
                        }
                        Dispatch(Events.ReplaceItem, (oldItem, newItem));
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual bool RemoveItem(Item item)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                if (!slot.Lock)
                {
                    if (slot.RemoveItem(item))
                    {
                        if (slot.Item == null)
                        {
                            FreeSlots.Add(slot);
                        }
                        Dispatch(Events.RemoveItem, item);
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool AddSlot(InventorySlot slot)
        {
            if (slot != null)
            {
                if (slot.Item == null)
                {
                    FreeSlots.Add(slot);
                }
                slot.AddRule(CheckItem);
                Slots.Add(slot);
                Dispatch(Events.AddSlot, slot);
            }
            return false;
        }
        public virtual bool RemoveSlot(InventorySlot slot)
        {
            if (slot != null)
            {
                if (Slots.Remove(slot))
                {
                    FreeSlots.Remove(slot);
                    slot.RemoveRule(CheckItem);
                    Dispatch(Events.RemoveSlot, slot);
                    return true;
                }
            }
            return false;
        }

        public virtual IEnumerator<InventorySlot> GetSlots()
        {
            return Slots.GetEnumerator();
        }
        public virtual IEnumerator<Item> GetItems()
        {
            return new InventoryEnumerator(GetSlots());
        }
    }
}