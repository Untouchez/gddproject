using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Dialogues;
using UnityEngine.UI;

namespace GDDProject.UI
{
    public class UIToggle : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.I;
        [SerializeField] GameObject uiContainer = null;

        // TO BE MOVED
        private Player player = null;
        private CameraFollow cam = null;

        void Start()
        {
            uiContainer.SetActive(false);
            player = FindObjectOfType<Player>();
            cam = FindObjectOfType<CameraFollow>();
            player.GetComponent<PlayerConversant>().onConversationUpdated += HideUI;
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && player.GetComponent<PlayerConversant>().GetCurrentAIConversant() == null)
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);

            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf && child.gameObject != uiContainer
                    && !child.GetComponent<Text>())
                {
                    return;
                }
            }

            player.StopPlayer();
            
            player.enabled = !player.enabled;
            cam.enabled = !cam.enabled;          
        }

        public void HideUI()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Dialogue")
                {
                    continue;
                }
                child.gameObject.SetActive(false);
            }
        }
    }
}