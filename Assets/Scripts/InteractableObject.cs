using GDDProject.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GDDProject.Core
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private Text interactUI = null;
        [SerializeField] private Text failureUI = null;
        [SerializeField] private KeyCode keyToInteract = KeyCode.E;

        private bool toBeDestoryed = false;

        private void Update()
        {
            if (interactUI.gameObject.activeSelf && Input.GetKeyDown(keyToInteract))
            {
                bool success = GetComponent<Pickup>().PickUp();
                interactUI.gameObject.SetActive(false);
                if (!success)
                {
                    // spawn "unable to pick up item text"
                    failureUI.gameObject.SetActive(true);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                toBeDestoryed = false;
                other.gameObject.GetComponent<InteractableDetector>().EnableNearestInteractableText();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                toBeDestoryed = true;
                HideText();
                other.gameObject.GetComponent<InteractableDetector>().EnableNearestInteractableText();
            }
        }

        private void OnDestroy()
        {
            toBeDestoryed = true;
            var detector = FindObjectOfType<InteractableDetector>();
            if (detector != null)
            {
                detector.EnableNearestInteractableText();
            }
        }

        public void ShowText()
        {
            interactUI.text = "Press " + keyToInteract.ToString() + " to pick up";
            if (failureUI.gameObject.activeSelf)
            {
                return;
            }
            interactUI.gameObject.SetActive(true);
        }

        public void HideText()
        {
            
            interactUI.gameObject.SetActive(false);
            failureUI.gameObject.SetActive(false);
            
        }

        public bool IsToBeDestroyed()
        {
            return toBeDestoryed;
        }
    }
}