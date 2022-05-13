using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int clericSkillPoints = 3;
    private int aerisSkillPoints = 3;
    private int samaraSkillPoints = 3;

    [SerializeField] private AudioManager audioManager;

    public static string currentSceneName {get; private set;}

    #region Scene Name Strings
        public const string TITLE_SCENE_NAME = "MainMenu";

        public const string COMBAT_SCENE_NAME = "Chase's Sandbox";

        public const string PROLOGUE_SCENE_NAME = "Prologue";
        public const string PROLOGUE_2_SCENE_NAME = "Prologue2";

        public const string AE_SCENE_1_NAME = "";
        public const string AE_SCENE_2_NAME = "";
        public const string AE_SCENE_3_NAME = "";
        public const string AE_EPILOGUE_SCENE_NAME = "";

        public const string SA_SCENE_1_NAME = "";
        public const string SA_SCENE_2_NAME = "";
        public const string SA_SCENE_3_NAME = "";
        public const string SA_EPILOGUE_SCENE_NAME = "";
    #endregion

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject); 
        SceneManager.sceneLoaded += OnSceneLoad;

        clericSkillPoints = 3;
        aerisSkillPoints = 3;
        samaraSkillPoints = 3;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        if(SceneHasFungus()){
            UpdatePlayerNameInFungus();
        }

        audioManager.StopAllTracks();
        if(currentSceneName == COMBAT_SCENE_NAME){
            audioManager.Play( AudioManager.NORMAL_FIGHT_MUSIC );
            // TODO: pass in the enemies/encounter data based on where we're at in the story
        }
        else if(currentSceneName == PROLOGUE_SCENE_NAME){
            audioManager.Play( AudioManager.AMBIENT_MUSIC );
        }
        else if(currentSceneName == PROLOGUE_2_SCENE_NAME){
            audioManager.Play( AudioManager.AMBIENT_MUSIC );
        }

        else if(currentSceneName == TITLE_SCENE_NAME){
            audioManager.Play( AudioManager.TITLE_MUSIC );
        }
    }

    public void EndCombat()
    {
        // TEMP FOR PROTOTYPE
        SceneManager.LoadScene(PROLOGUE_2_SCENE_NAME);
    }

    public int GetCurrentSkillPoints(EntityID characterID)
    {
        switch(characterID)
        {
            case EntityID.Aeris:
                return aerisSkillPoints;
            case EntityID.MainCharacter:
                return clericSkillPoints;
            case EntityID.Samara:
                return samaraSkillPoints;
            default:
                Debug.LogWarning("Tried to get skill points for an ID that was not a character.");
                return -1;
        }
    }

    public void SpendSkillPointForCharacter(EntityID characterID)
    {
        switch(characterID)
        {
            case EntityID.Aeris:
                if(aerisSkillPoints > 0)
                    --aerisSkillPoints;
                else
                    Debug.LogWarning("Tried to spend skill points for a character that has none left.");
                break;
            case EntityID.MainCharacter:
                if(clericSkillPoints > 0)
                    --clericSkillPoints;
                else
                    Debug.LogWarning("Tried to spend skill points for a character that has none left.");
                break;
            case EntityID.Samara:
                if(samaraSkillPoints > 0)
                    --samaraSkillPoints;
                else
                    Debug.LogWarning("Tried to spend skill points for a character that has none left.");
                break;
            default:
                Debug.LogWarning("Tried to spend skill points for an ID that was not a character.");
                break;
        }
    }

    public void AddSkillPointForCharacter(EntityID characterID)
    {
        switch(characterID)
        {
            case EntityID.Aeris:
                if(aerisSkillPoints < 3)
                    ++aerisSkillPoints;
                else
                    Debug.LogWarning("Tried to add skill points for a character that has the max.");
                break;
            case EntityID.MainCharacter:
                if(clericSkillPoints < 3)
                    ++clericSkillPoints;
                else
                    Debug.LogWarning("Tried to add skill points for a character that has the max.");
                break;
            case EntityID.Samara:
                if(samaraSkillPoints < 3)
                    ++samaraSkillPoints;
                else
                    Debug.LogWarning("Tried to add skill points for a character that has the max.");
                break;
            default:
                Debug.LogWarning("Tried to spend skill points for an ID that was not a character.");
                break;
        }
    }

    #region Fungus Stuff
        public bool SceneHasFungus()
        {
            if( currentSceneName == TITLE_SCENE_NAME || currentSceneName == COMBAT_SCENE_NAME ){
                return false;
            }
            else{
                return true;
            }
        }

        public void UpdatePlayerNameInFungus()
        {
            if( !SceneHasFungus() ){
                return;
            }

            // update name for fungus
            Flowchart fc = GameObject.FindObjectOfType<Flowchart>();

            fc.SetStringVariable("Name", Settings.playerName);
        }
    #endregion
}
