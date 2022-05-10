using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static string currentSceneName {get; private set;}

    #region Scene Name Strings
        public const string TITLE_SCENE_NAME = "MainMenu";

        public const string PROLOGUE_SCENE_NAME = "Prologue";
        public const string COMBAT_SCENE_NAME = "";

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

        if(currentSceneName == PROLOGUE_SCENE_NAME){
            // TODO
        }
        else if(currentSceneName == COMBAT_SCENE_NAME){
            // TODO: pass in the enemies based on where we're at in the story
        }
    }

}
