using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDetector : MonoBehaviour
{
    [SerializeField] private float interactableDetectionRadius = 1.0f;
    [SerializeField] private LayerMask interactableLayerMask;

    public void EnableNearestInteractableText()
    {
        Collider[] hits = 
            Physics.OverlapSphere(transform.position, interactableDetectionRadius,
            interactableLayerMask);
        if (hits.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            GameObject closestInteractable = null;
            foreach (var hit in hits)
            {
                if (hit.GetComponent<IInteractable>().IsToBeDestroyed())
                {
                    continue;
                }
                if (Vector3.Distance(hit.transform.position, transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(hit.transform.position, transform.position);
                    closestInteractable = hit.gameObject;
                }
            }

            foreach (var hit in hits)
            {
                if (closestInteractable == null || closestInteractable != hit.gameObject)
                {
                    hit.GetComponent<IInteractable>().HideText();
                }
            }

            if (closestInteractable != null)
            {
                Debug.Log(closestInteractable.name);
                IInteractable interactable = closestInteractable.GetComponent<IInteractable>();
                interactable.ShowText();
            }           
        }
    }
}
