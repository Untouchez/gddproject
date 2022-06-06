using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
            player.enabled = !player.enabled;
            cam.enabled = !cam.enabled;
        }
    }
}