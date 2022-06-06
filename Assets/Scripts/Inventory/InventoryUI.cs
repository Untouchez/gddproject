using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Inventories;

namespace GDDProject.UI.Inventories
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] GameObject inventorySlotUI = null;

        Inventory playerInventory;

        private void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.inventoryUpdated += UpdateUI;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(inventorySlotUI, transform);
                itemUI.GetComponentInChildren<InventoryItemUI>().SetItem(playerInventory.GetItemInSlot(i));
            }
        }
    }
}
