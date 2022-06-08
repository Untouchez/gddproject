using GDDProject.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Quests
{
    public class QuestList : MonoBehaviour, IDialogueNodeConditionEvaluator
    {
        private List<QuestProgress> progresses = new List<QuestProgress>();

        public event Action onQuestListUpdated;

        public List<QuestProgress> GetProgresses()
        {
            return progresses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
            {
                return;
            }

            QuestProgress progress = new QuestProgress(quest);
            progresses.Add(progress);
            if (onQuestListUpdated != null)
            {
                onQuestListUpdated();
            }
        }

        public void CompleteQuest(Quest quest)
        {
            QuestProgress progress = GetQuestProgress(quest);
            if (progress != null)
            {
                progress.Complete();
                GiveReward(quest);
               
                if (onQuestListUpdated != null)
                {
                    onQuestListUpdated();
                }
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestProgress(quest) != null;
        }

        private QuestProgress GetQuestProgress(Quest quest)
        {
            foreach (QuestProgress progress in progresses)
            {
                if (progress.GetQuest() == quest)
                {
                    return progress;
                }
            }

            return null;
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.itemReward);
                if (!success)
                {
                    // spawn a pickup instead!
                    GetComponent<PickupSpawner>().SpawnItem(reward.itemReward);
                }
            }
        }

        public bool? Evaluate(Condition condition, IEnumerable<UnityEngine.Object> objects)
        {
            foreach (var obj in objects)
            {
                Quest quest = obj as Quest;
                
                if (quest != null)
                {
                    switch (condition)
                    {
                        case Condition.HasQuest:
                            return HasQuest(quest);
                        case Condition.CompletedQuest:
                            if (!HasQuest(quest))
                            {
                                return false;
                            }
                            return GetQuestProgress(quest).IsCompleted();
                    }
                }
            }
            return true;
        }
    }
}
