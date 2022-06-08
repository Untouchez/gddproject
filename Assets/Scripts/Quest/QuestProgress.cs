using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Quests
{
    public class QuestProgress
    {
        private Quest quest;
        private bool completed = false;

        public QuestProgress(Quest quest)
        {
            this.quest = quest;
        }

        public void Complete()
        {
            completed = true;
        }

        public bool IsCompleted()
        {
            return completed;
        }

        public Quest GetQuest()
        {
            return quest;
        }
    }
}