using GDDProject.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueNodeConditionEvaluator
{
    bool? Evaluate(Condition condition, IEnumerable<UnityEngine.Object> objects);
}
