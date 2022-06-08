using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest quest = null;

        public void GiveQuest()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>().AddQuest(quest);
        }
    }
}
