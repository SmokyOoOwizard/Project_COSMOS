using System.Collections;
using System.Collections.Generic;

namespace COSMOS.Common.Inventory
{
    public class InventoryEnumerator : IEnumerator<Item>
    {
        private IEnumerator<InventorySlot> slotsEnumerator;

        public InventoryEnumerator(IEnumerator<InventorySlot> enumerator)
        {
            this.slotsEnumerator = enumerator;
        }

        public Item Current
        {
            get
            {
                if (slotsEnumerator.Current == null)
                {
                    return null;
                }
                return slotsEnumerator.Current.Item;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (slotsEnumerator.Current == null)
                {
                    return null;
                }
                return slotsEnumerator.Current.Item;
            }
        }

        public void Dispose()
        {
            slotsEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return slotsEnumerator.MoveNext();
        }

        public void Reset()
        {
            slotsEnumerator.Reset();
        }
    }
}