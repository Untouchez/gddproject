using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDDProject.Dialogues
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string playerName;

        private Dialogue currentDialogue;
        private DialogueNode currentNode;
        private bool isChoosing = false;
        private AIConversant currentAIConversant;

        // event subscribed by DialogueUI's UpdateUI(), so that when StartDialogue() executes, UpdateUI()
        // will be called, without circular dependency
        public event Action onConversationUpdated;

        // TO BE DELETED
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var NPC = FindObjectOfType<AIConversant>();
                NPC.TalkToPlayer();
            }
        }

        public void StartDialogue(Dialogue newDialogue, AIConversant conversant)
        {
            currentDialogue = newDialogue;
            currentAIConversant = conversant;
            // initialize currentNode before it is accessed by DialogueUI
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();

            // TO BE MOVED
            GetComponent<Player>().enabled = false;
            FindObjectOfType<CameraFollow>().enabled = false;
        }

        // accessed by DialogueUI, returning the current dialogue text to be displayed
        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();
        }

        // accessed by DialogueUI, returning an IEnumerable of choices as strings to be displayed
        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentAIConversant.GetName();
            }
        }

        // accessed by DialogueUI, progresses down a player choice in the dialogue tree
        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            if (HasNext())
            {
                Next();
            }
            else
            {
                Quit();
            }
        }

        // accessed by DialogueUI, progresses down the dialogue tree when Next button is pressed
        public void Next()
        {
            // check if Next() results in player choices
            // find the number of player dialogue nodes after currentNode
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 0)
            {
                // player choosing state!
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            // no player dialogue nodes after currentNode
            // retrieve all the children (AI response nodes) of currentNode as an array instead of IEnumerable
            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            // pick a random response to be displayed
            TriggerExitAction();
            currentNode = children[UnityEngine.Random.Range(0, children.Count())];
            TriggerEnterAction();
            isChoosing = false;
            onConversationUpdated();
        }

        // accessed by DialogueUI, quits the current dialogue
        public void Quit()
        {
            // reset all the states
            currentDialogue = null;
            TriggerExitAction();
            currentAIConversant = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated();

            // TO BE MOVED
            GetComponent<Player>().enabled = true;
            FindObjectOfType<CameraFollow>().enabled = true;
        }

        // accessed by DialogueUI, checks whether we have reached the end of a tree branch
        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        // accessed by DialogueUI, checks whether we have a non-null/active currentDialogue object
        public bool IsActive()
        {
            return currentDialogue != null;
        }

        // accessed by DialogueUI, checks whether we are currently in AI response or player choice state
        public bool IsChoosing()
        {
            return isChoosing;
        }

        public AIConversant GetCurrentAIConversant()
        {
            return currentAIConversant;
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                // we have a current node and an action on entering that node
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                // we have a current node and an action on exiting that node
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "")
            {
                return;
            }

            foreach (DialogueTrigger trigger in currentAIConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
