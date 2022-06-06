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

    private int aerisRouteProgression = 0;
    private int aerisAffection = 0;
    private int samaraRouteProgression = 0;
    private int samaraAffection = 0;


    [SerializeField] private AudioManager audioManager;

    public static string currentSceneName {get; private set;}

    #region Scene Name Strings
        public const string TITLE_SCENE_NAME = "MainMenu";

        public const string COMBAT_SCENE_NAME = "Chase's Sandbox";

        public const string PROLOGUE_SCENE_NAME = "Prologue";
        public const string PROLOGUE_2_SCENE_NAME = "Prologue2";

        public const string AE_SCENE_1_NAME = "Aeris1";
        public const string AE_SCENE_2_NAME = "Aeris2";
        // public const string AE_SCENE_3_NAME = "";
        public const string AE_EPILOGUE_SCENE_NAME = "AerisEndings";

        public const string SA_SCENE_1_NAME = "Samara1";
        public const string SA_SCENE_2_NAME = "Samara2";
        // public const string SA_SCENE_3_NAME = "";
        public const string SA_EPILOGUE_SCENE_NAME = "SamaraEndings";
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
        // Debug.Log(currentSceneName);

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
        if (aerisRouteProgression == 0 && samaraRouteProgression == 0) // Neither route has been selected yet
            SceneManager.LoadScene(PROLOGUE_2_SCENE_NAME);
        else if (aerisRouteProgression == 1 && samaraRouteProgression == 0) // Aeris was picked, go to first scene
            SceneManager.LoadScene(AE_SCENE_1_NAME);
        else if (aerisRouteProgression == 2 && samaraRouteProgression == 0) // Aeris's first scene completed, go to second
            SceneManager.LoadScene(AE_SCENE_2_NAME);
        else if (aerisRouteProgression == 3 && samaraRouteProgression == 0) // Aeris's second scene completed, go to endings
            SceneManager.LoadScene(AE_EPILOGUE_SCENE_NAME);
        else if (aerisRouteProgression == 0 && samaraRouteProgression == 1) // Samara was picked, go to first scene
            SceneManager.LoadScene(SA_SCENE_1_NAME);
        else if (aerisRouteProgression == 0 && samaraRouteProgression == 2) // Samara's first scene completed, go to second
            SceneManager.LoadScene(SA_SCENE_2_NAME);
        else if (aerisRouteProgression == 0 && samaraRouteProgression == 3) // Samara's second scene completed, go to endings
            SceneManager.LoadScene(SA_EPILOGUE_SCENE_NAME);
    }

    #region Skill Point Management
        public void CharacterRest()
        {
            if(aerisSkillPoints < 3)
                AddSkillPointForCharacter(EntityID.Aeris);
            if(clericSkillPoints < 3)
                AddSkillPointForCharacter(EntityID.MainCharacter);
            if(samaraSkillPoints < 3)
                AddSkillPointForCharacter(EntityID.Samara);
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
    #endregion

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

        public void UpdateFungusValues()
        {
            UpdatePlayerNameInFungus();
            UpdatePlayerPronounsInFungus();
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

        public void UpdatePlayerPronounsInFungus()
        {
            if( !SceneHasFungus() ){
                return;
            }

            Flowchart fc = GameObject.FindObjectOfType<Flowchart>();
            fc.SetStringVariable("mc_pronoun", Settings.pronouns.ToString());

            GenderedTerms.GenderedTermLoader gtl = GameObject.FindObjectOfType<GenderedTerms.GenderedTermLoader>();
            if(gtl){
                gtl.UpdateAllGenderedTermVariables();
            }
        }

        public void SetAerisGoodEnding()
        {
            if (!SceneHasFungus())
            {
                return;
            }

            // update bool for fungus
            Flowchart fc = GameObject.FindObjectOfType<Flowchart>();

            bool unlocked = aerisAffection == 2;
            fc.SetBooleanVariable("GoodEndingUnlocked", unlocked);
        }

        public void SetSamaraGoodEnding()
        {
            if (!SceneHasFungus())
            {
                return;
            }

            // update bool for fungus
            Flowchart fc = GameObject.FindObjectOfType<Flowchart>();

            bool unlocked = samaraAffection == 2;
            fc.SetBooleanVariable("GoodEndingUnlocked", unlocked);
        }

        public void IncrementAerisRoute()
        {
            aerisRouteProgression++;
        }

        public void GrowCloserToAeris()
        {
            aerisAffection++;
        }

        public void IncrementSamaraRoute()
        {
            aerisRouteProgression++;
        }

        public void GrowCloserToSamara()
        {
            samaraAffection++;
        }

        public int AerisAffection => aerisAffection;

        public int SamaraAffection => samaraAffection;
    #endregion
}
