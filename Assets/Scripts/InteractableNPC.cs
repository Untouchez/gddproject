using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Dialogues;
using UnityEngine.UI;

namespace GDDProject.Core
{
    public class InteractableNPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private Text interactUI = null;
        [SerializeField] private KeyCode keyToInteract = KeyCode.E;

        private PlayerConversant playerConversant;
        private bool toBeDestroyed = false;

        private void Start()
        {
            playerConversant = FindObjectOfType<PlayerConversant>();
            playerConversant.onConversationUpdated += EnableInstructionText;
        }

        private void Update()
        {
            if (interactUI.gameObject.activeSelf && Input.GetKeyDown(keyToInteract))
            {
                GetComponent<AIConversant>().TalkToPlayer();
                interactUI.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                toBeDestroyed = false;
                other.gameObject.GetComponent<InteractableDetector>().EnableNearestInteractableText();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                toBeDestroyed = true;
                HideText();
                other.gameObject.GetComponent<InteractableDetector>().EnableNearestInteractableText();
            }
        }

        private void OnDestroy()
        {
            toBeDestroyed = true;
            var detector = FindObjectOfType<InteractableDetector>();
            if (detector != null) {
                detector.EnableNearestInteractableText();
            }
        }

        // makes sure that when player finishes a dialogue, the instruction text shows up
        private void EnableInstructionText()
        {
            if (playerConversant.GetCurrentAIConversant() == null)
            {
                interactUI.gameObject.SetActive(true);
            }
        }

        public void ShowText()
        {
            interactUI.text = "Press " + keyToInteract.ToString() + " to talk";
            interactUI.gameObject.SetActive(true);
        }

        public void HideText()
        {

            interactUI.gameObject.SetActive(false);

        }

        public bool IsToBeDestroyed()
        {
            return toBeDestroyed;
        }
    }
}
