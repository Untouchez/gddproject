using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Dialogues
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue = null;
        [SerializeField] private string conversantName;

        public string GetName()
        {
            return conversantName;
        }

        public void TalkToPlayer()
        {
            FindObjectOfType<PlayerConversant>().StartDialogue(dialogue, this);
        }

        public void Boo()
        {
            Debug.Log("Boo");
        }
    }
}
