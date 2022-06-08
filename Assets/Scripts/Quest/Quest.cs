using GDDProject.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDDProject.Quests 
{
    [CreateAssetMenu(fileName = "Quest", menuName = "GDDProject/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Reward> rewards = new List<Reward>();
        [SerializeField] private string objective;

        [System.Serializable]
        public class Reward
        {
            public InventoryItem itemReward;
        }

        public string GetTitle()
        {
            return name;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public string GetObjective()
        {
            return objective;
        }

        public bool HasObjective(string objective)
        {
            return objective == this.objective;
        }

        public static Quest GetQuestByName(string questName)
        {
            // get a quest scriptable object by name
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }

            return null;
        }
    }
}
