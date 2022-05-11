using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
            Character[] characters = GameObject.FindObjectsOfType<Character>();

            foreach (Character character in characters)
            {
                if (character.tag == "Player")
                {
                    character.NameText = Settings.playerName;
                }
            }

            fc.SetStringVariable("Name", Settings.playerName);
        }
    #endregion
}
