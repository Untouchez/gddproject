using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;

        public void CompleteQuest()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>().CompleteQuest(quest);
        }
    }
}
