using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Inventories
{
    public class Inventory : MonoBehaviour, IDialogueNodeConditionEvaluator
    {
        [SerializeField] int inventorySize = 16;

        [SerializeField] private InventoryItem[] slots;

        public event Action inventoryUpdated;

        public static Inventory GetPlayerInventory()
        {
            var player = FindObjectOfType<Inventory>();
            return player;
        }

        public bool HasSpaceFor(InventoryItem item)
        {
            if (item == null)
            {
                return false;
            }
            return FindEmptySlot() >= 0;
        }

        public int GetSize()
        {
            return slots.Length;
        }

        public bool AddToFirstEmptySlot(InventoryItem item)
        {
            int i = FindEmptySlot();

            if (i < 0)
            {
                return false;
            }

            slots[i] = item;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot];
        }

        public void RemoveFromSlot(int slot)
        {
            slots[slot] = null;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        public bool AddItemToSlot(int slot, InventoryItem item)
        {
            if (slots[slot] != null)
            {
                return AddToFirstEmptySlot(item); ;
            }

            slots[slot] = item;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        private void Awake()
        {
            //slots = new InventoryItem[inventorySize];
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool? Evaluate(Condition condition, IEnumerable<UnityEngine.Object> objects)
        {
            if (condition != Condition.HasInventoryItem)
            {
                return null;
            }

            foreach (var obj in objects)
            {
                InventoryItem item = obj as InventoryItem;
                if (item != null)
                {
                    return HasItem(item);
                }
            }

            // no required inventory item supplied
            return true;
        }
    }
}
 