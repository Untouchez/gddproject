using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Inventories;
using UnityEngine.UI;

namespace GDDProject.UI.Inventories
{
    public class InventoryItemUI : MonoBehaviour
    {
        public void SetItem(InventoryItem item)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }
        }
    }
}
