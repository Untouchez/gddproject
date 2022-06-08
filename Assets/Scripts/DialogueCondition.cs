using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Inventories;
using GDDProject.Quests;

[System.Serializable]
public enum Condition
{
    HasInventoryItem,
    HasQuest,
    CompletedQuest,
    None
}

[System.Serializable]
public class DialogueCondition
{
    [SerializeField] private Disjunction[] ands;

    public bool CheckAllEvaluators(IEnumerable<IDialogueNodeConditionEvaluator> evaluators)
    {
        foreach (var disj in ands)
        {
            if (!disj.CheckAllEvaluators(evaluators))
            {
                return false;
            }
        }
        return true;
    }

    [System.Serializable]
    class Disjunction
    {
        [SerializeField] private ConditionContainer[] ors;

        public bool CheckAllEvaluators(IEnumerable<IDialogueNodeConditionEvaluator> evaluators)
        {
            foreach (var container in ors)
            {
                if (container.CheckAllEvaluators(evaluators))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [System.Serializable]
    class ConditionContainer
    {
        [SerializeField] private Condition condition;
        [SerializeField] private bool negated;
        [SerializeField] private Object[] objects;
        /*public InventoryItem item;
        public Quest quest;*/

        public bool CheckAllEvaluators(IEnumerable<IDialogueNodeConditionEvaluator> evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                bool? result = evaluator.Evaluate(condition, objects);
                if (result == null)
                {
                    continue;
                }
                if (result == negated)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
