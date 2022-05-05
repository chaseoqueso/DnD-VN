using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerPronouns{
    she,    // she/her/hers
    he,     // he/him/his
    they,    // they/them/theirs
    none   // default when you start a new game
}

public class Settings : MonoBehaviour
{
    public const string PLAYER_NAME_KEY = "Name";
    public const string PLAYER_PRONOUN_KEY = "Pronouns";
    
    public static bool disableUnpauseUntilSettingsSelected;

    public static PlayerPronouns pronouns {get; private set;}
    public static string playerName {get; private set;}

    [Tooltip("Back button if in game, play if main menu")]
    public Button backButton;

    public TMP_InputField nameInputField;
    public List<Toggle> pronounToggles = new List<Toggle>();

    void Start()
    {
        LoadSaveData();

        // If the pronoun toggle is changed, take note of that
        for(int i = 0; i < pronounToggles.Count; i++){
            pronounToggles[i].onValueChanged.AddListener( delegate{
                for(int j = 0; j < pronounToggles.Count; j++){
                    if(pronounToggles[j].isOn){
                        SetPlayerPronouns((PlayerPronouns)j);
                    }
                }
                ToggleBackButtonInteractableBasedOnSettingStatus();
            });
        }

        // If the value in the textbox is changed, disable the back button if it's blank
        nameInputField.onValueChanged.AddListener(delegate{
            ToggleBackButtonInteractableBasedOnSettingStatus();
        });

        ToggleBackButtonInteractableBasedOnSettingStatus();
    }

    public void ToggleBackButtonInteractableBasedOnSettingStatus()
    {
        if(!NameTextIsValid() || pronouns == PlayerPronouns.none){
            backButton.interactable = false;
            disableUnpauseUntilSettingsSelected = true;
        }
        else{
            backButton.interactable = true;
            disableUnpauseUntilSettingsSelected = false;
        }
    }

    public bool NameTextIsValid()
    {
        string text = nameInputField.text.ToLower();
        Debug.Log(text);
        if(text == "" || text == "aeris" || text == "samara"){
            // TODO: UI feedback that the name is invalid
            return false;
        }
        return true;
    }

    public void LoadSaveData()
    {
        if(PlayerPrefs.HasKey(PLAYER_NAME_KEY)){
            playerName = PlayerPrefs.GetString(PLAYER_NAME_KEY);
            pronouns = (PlayerPronouns)PlayerPrefs.GetInt(PLAYER_PRONOUN_KEY);
            nameInputField.text = playerName;
        }
        else{
            // Default to none if nothing is set, at the start of a new game make players pick
            pronouns = PlayerPronouns.none;
            playerName = "";
        }
    }

    public void SavePlayerNameOnButtonClicked()
    {
        // Set player name to whatever is currently in the text box
        SetPlayerName(nameInputField.text);
    }

    public void SetPlayerName(string _name)
    {
        playerName = _name;
        Debug.Log("Name set to: " + playerName);
        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
        PlayerPrefs.Save();
    }

    public void SetPlayerPronouns(PlayerPronouns _pronouns)
    {
        pronouns = _pronouns;
        Debug.Log("Pronouns set to: " + pronouns);
        PlayerPrefs.SetInt(PLAYER_PRONOUN_KEY, (int)pronouns);
        PlayerPrefs.Save();
    }

    public static string ReplacePronounCodesInString(string s)
    {
        string newString = "";

        if( pronouns == PlayerPronouns.she ){
            // TODO: swap the {1/2/3} for the FIRST option in the {}
        }
        else if(pronouns == PlayerPronouns.he){
            // TODO: swap the {1/2/3} for the SECOND option in the {}
        }
        else if(pronouns == PlayerPronouns.they){
            // TODO: swap the {1/2/3} for the THIRD option in the {}
        }
        else{
            Debug.LogError("No pronouns set! Cannot swap pronoun codes for pronouns in text!");
        }

        return newString;
    }
}
