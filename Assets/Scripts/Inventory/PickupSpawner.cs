using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Inventories
{
    public class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject pickupPrefab = null;
        [SerializeField] private Vector3 pickupSpawnOffset = new Vector3(0, 1, 0);

        public void SpawnItem(InventoryItem item)
        {
            var pickupInstance = Instantiate(pickupPrefab, transform.position + pickupSpawnOffset,
                Quaternion.identity);
            pickupInstance.GetComponent<Pickup>().SetItem(item);
        }
    }
}
