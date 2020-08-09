using System.Collections.Generic;
using System;

namespace COSMOS.Common.Inventory
{
    public class AutoExpandedInventory : Inventory
    {
        protected Func<InventorySlot> SlotFactory;

        protected readonly HashSet<InventorySlot> CreatedSlots = new HashSet<InventorySlot>();

        public AutoExpandedInventory()
        {
            SetSlotFactory(DefualtFactory);
        }

        public override bool AddItem(Item item)
        {
            if (FreeSlots.Count < 1)
            {
                var slot = SlotFactory();
                if (slot != null)
                {
                    if (AddSlot(slot))
                    {
                        CreatedSlots.Add(slot);
                    }
                }
            }
            if (base.AddItem(item))
            {
                var slot = SlotFactory();
                if (slot != null)
                {
                    if (AddSlot(slot))
                    {
                        CreatedSlots.Add(slot);
                    }
                }
                return true;
            }
            return false;
        }

        public override bool ReplaceItem(Item oldItem, Item newItem)
        {
            bool result = base.ReplaceItem(oldItem, newItem);
            if (newItem == null)
            {
                RemoveCreatedSlotsTail();
            }
            return result;
        }
        public override bool RemoveItem(Item item)
        {
            bool result = base.RemoveItem(item);

            RemoveCreatedSlotsTail();

            return result;
        }

        protected void RemoveCreatedSlotsTail()
        {
            for (int i = Slots.Count - 1; i > 0; i--)
            {
                var slot = Slots[i];
                if (slot.Item == null && Slots[i - 1].Item == null)
                {
                    if (CreatedSlots.Remove(slot))
                    {
                        RemoveSlot(slot);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public void SetSlotFactory(Func<InventorySlot> factory)
        {
            if (factory != null)
            {
                SlotFactory = factory;
            }
        }

        public static InventorySlot DefualtFactory()
        {
            return new InventorySlot();
        }
    }
}