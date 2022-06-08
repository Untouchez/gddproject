using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDDProject.Dialogues
{
    public class DialogueNode : ScriptableObject
    {
        // mark these properties as [SerializeField] so that they become private and can be serialized 
        // and saved onto Undo stack
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] bool isPlayerSpeaking = false;
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private DialogueCondition condition;

        // the DialogueNode class should handle its own Undo's for its states
        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren() 
        {
            return children;
        }

        public Rect GetRect()
        {
            return rect;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool PassCondition(IEnumerable<IDialogueNodeConditionEvaluator> evaluators)
        {
            return condition.CheckAllEvaluators(evaluators);
        }

// UNITY_EDITOR macro is used for excluding all the code that can change our dialogue editor from the final build
// because we cannot really change our dialogue editor in the build of the game (we only do so when developing)
#if UNITY_EDITOR
        public void SetRectPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Update Node Position");
            rect.position = position;
            // to resolve a bug where subassets (DialogueNode) of a scriptable object (Dialogue)
            // are not saved properly by Unity
            EditorUtility.SetDirty(this);
        }

        public void SetText(string text)
        {
            // check if the text inside this node actually changed
            if (text != this.text)
            {
                Undo.RecordObject(this, "Update Node Text");
                this.text = text;
                EditorUtility.SetDirty(this);
            }            
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Added Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Removed Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "Update Dialogue Speaker");
            this.isPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
