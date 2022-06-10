using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public bool gameIsPaused {get; private set;}

    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private GameObject pauseMenuDefaultPanel;
    [SerializeField] private GameObject settingsMenuPanel;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsBackButton;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        gameIsPaused = false;
    }

    public void ResumeGame()
    {
        if(Settings.disableUnpauseUntilSettingsSelected){
            return;
        }

        ResetPauseUI();
        Time.timeScale = 1f;
        gameIsPaused = false;
        
        UIManager.instance.SetButtonsInteractable(true);
    }

    private void ResetPauseUI()
    {
        settingsMenuPanel.SetActive(false);
        pauseMenuDefaultPanel.SetActive(true);

        pauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        continueButton.Select();
        gameIsPaused = true;

        UIManager.instance.SetButtonsInteractable(false);

        if(Settings.disableUnpauseUntilSettingsSelected){
            ToggleSettingsMenu(true);
        }
    }

    public void LoadMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        
        if( GameManager.instance.SceneHasFungus() ){
            // TODO: STOP FUNGUS AUDIO
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleSettingsMenu(bool set)
    {
        settingsMenuPanel.SetActive(set);
        pauseMenuDefaultPanel.SetActive(!set);

        if(set){
            settingsBackButton.Select();
        }
        else{
            continueButton.Select();
        }
    }
}
