using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerPronouns{
    SHE,    // she/her/hers
    HE,     // he/him/his
    THEY,    // they/them/theirs
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
    public Toggle shePronounToggle;
    public Toggle hePronounToggle;
    public Toggle theyPronounToggle;

    void Start()
    {
        LoadSaveData();

        // If the pronoun toggle is changed, take note of that
        shePronounToggle.onValueChanged.AddListener( (bool value) => UpdatePlayerPronounToggleValue(PlayerPronouns.SHE, value) );
        hePronounToggle.onValueChanged.AddListener( (bool value) => UpdatePlayerPronounToggleValue(PlayerPronouns.HE, value) );
        theyPronounToggle.onValueChanged.AddListener( (bool value) => UpdatePlayerPronounToggleValue(PlayerPronouns.THEY, value) );

        // If the value in the textbox is changed, disable the back button if it's blank
        nameInputField.onValueChanged.AddListener(delegate{
            ToggleBackButtonInteractableBasedOnSettingStatus();
        });

        ToggleBackButtonInteractableBasedOnSettingStatus();

        GameManager.instance.UpdateFungusValues();
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
        if(text == "" || text == "aeris" || text == "samara"){
            return false;
        }
        return true;
    }

    public void LoadSaveData()
    {
        if(PlayerPrefs.HasKey(PLAYER_NAME_KEY) && PlayerPrefs.HasKey(PLAYER_PRONOUN_KEY)){
            playerName = PlayerPrefs.GetString(PLAYER_NAME_KEY);
            nameInputField.text = playerName;

            pronouns = (PlayerPronouns)PlayerPrefs.GetInt(PLAYER_PRONOUN_KEY);
            switch(pronouns){
                case PlayerPronouns.SHE:
                    shePronounToggle.isOn = true;
                    break;
                case PlayerPronouns.HE:
                    hePronounToggle.isOn = true;
                    break;
                case PlayerPronouns.THEY:
                    theyPronounToggle.isOn = true;
                    break;
            }
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
        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
        PlayerPrefs.Save();

        GameManager.instance.UpdatePlayerNameInFungus();

        if( GameManager.currentSceneName == GameManager.PROLOGUE_COMBAT_SCENE_NAME ){
            UIManager.instance.combatUI.UpdateMainCharacterName();
        }
    }

    public void UpdatePlayerPronounToggleValue(PlayerPronouns _pronouns, bool value)
    {
        if(value){
            pronouns = _pronouns;
            // Debug.Log("Pronouns set to: " + pronouns);
            PlayerPrefs.SetInt(PLAYER_PRONOUN_KEY, (int)pronouns);
            PlayerPrefs.Save();

            GameManager.instance.UpdatePlayerPronounsInFungus();
        }
    }

    public static string ReplacePronounCodesInString(string s)
    {
        string newString = "";

        if( pronouns == PlayerPronouns.SHE ){
            // TODO: swap the {1/2/3} for the FIRST option in the {}
        }
        else if(pronouns == PlayerPronouns.HE){
            // TODO: swap the {1/2/3} for the SECOND option in the {}
        }
        else if(pronouns == PlayerPronouns.THEY){
            // TODO: swap the {1/2/3} for the THIRD option in the {}
        }
        else{
            Debug.LogError("No pronouns set! Cannot swap pronoun codes for pronouns in text!");
        }

        return newString;
    }
}
