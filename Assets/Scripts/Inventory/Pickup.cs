using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Inventories
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private InventoryItem item = null;

        private Inventory inventory = null;

        private void Start()
        {
            inventory = FindObjectOfType<Inventory>();
        }

        public void SetItem(InventoryItem item)
        {
            this.item = item;
        }

        public bool PickUp()
        {
            bool success = inventory.AddToFirstEmptySlot(item);
            if (!success)
            {
                return false;
            }
            else
            {
                Destroy(gameObject);
                return true;
            }
        }
    }
}
