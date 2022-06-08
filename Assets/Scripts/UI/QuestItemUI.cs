using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDDProject.Quests;
using UnityEngine.UI;

namespace GDDProject.UI
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private Text questName = null;
        [SerializeField] private Image checkBox = null;

        public void Setup(QuestProgress progress)
        {
            questName.text = progress.GetQuest().GetTitle();
            checkBox.gameObject.SetActive(progress.IsCompleted());
        }
    }
}
