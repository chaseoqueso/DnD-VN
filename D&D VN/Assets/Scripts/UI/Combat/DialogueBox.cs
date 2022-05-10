using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private Button progressButton;
    [SerializeField] private TMP_Text dialogueBoxText;

    private string currentDefaultDescription = "...";

    public delegate void ProgressButtonCallback();
    private ProgressButtonCallback buttonFunction;

    public static bool progressButtonIsActive;

    void Start()
    {
        ToggleProgressButton(false);
    }

    public void ToggleProgressButton(bool set)
    {
        progressButton.gameObject.SetActive(set);
        progressButtonIsActive = set;

        if(set){
            progressButton.Select();
        }
    }

    public void ToggleProgressButton(bool set, ProgressButtonCallback functionToPerform)
    {
        ToggleProgressButton(set);
        buttonFunction = functionToPerform;
    }

    public void OnButtonClicked()
    {
        if(buttonFunction == null){
            Debug.LogWarning("No function assigned to dialogue box progress button!");
            return;
        }
        buttonFunction.Invoke();
    }

    // Set as default state should be true if NOT messages revealed on hover/interactable select, just default combat state stuff like saying whose turn it is
    public void SetDialogueBoxText(string description, bool setAsDefaultState)
    {
        dialogueBoxText.text = description;

        if(setAsDefaultState){
            currentDefaultDescription = description;
        }
    }

    // Call when no longer hovering/selecting an interactable thing
    public void SetDialogueBoxToCurrentDefault()
    {
        dialogueBoxText.text = currentDefaultDescription;
    }
}
