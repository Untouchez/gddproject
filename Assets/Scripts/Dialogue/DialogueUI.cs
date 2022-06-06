using GDDProject.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GDDProject.UI.Dialogues
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private Text AIText = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private Button quitButton = null;
        [SerializeField] private Transform choiceRoot = null;
        [SerializeField] private GameObject AIResponse = null;
        [SerializeField] private GameObject choicePrefab = null;
        [SerializeField] private Text nameLabel = null;

        private PlayerConversant playerConversant;

        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            UpdateUI();

            // subscribe to button clicking event
            nextButton.onClick.AddListener(() =>
            {
                if (playerConversant.HasNext())
                {
                    playerConversant.Next();
                }
            });
            quitButton.onClick.AddListener(() =>
            {
                playerConversant.Quit();
            });
        }

        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }

            // disable AI response UI and enable player choice UI if we are in player choosing state
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            nameLabel.text = playerConversant.GetCurrentConversantName();

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            } 
            else
            {
                AIText.text = playerConversant.GetText();
                // disable nextButton if the dialogue is coming to an end
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            // destroy the buttons in choice UI
            foreach (Transform button in choiceRoot)
            {
                Destroy(button.gameObject);
            }
            // recreate the buttons in choice UI
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                choiceInstance.GetComponentInChildren<Text>().text = choice.GetText();
                // attach a lambda function as a listener to each button
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    // since we cannot specify a parameter to be passed in if we add listener in the usual
                    // way button.onClick.AddListener(funcName), we will create a lambda function that 
                    // takes in no argument and instead calls PlayerConversant.SelectChoice(), passing in
                    // a DialogueNode as argument
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
