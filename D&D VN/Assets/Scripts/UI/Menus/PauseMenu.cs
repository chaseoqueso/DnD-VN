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
        ResetPauseUI();
        Time.timeScale = 1f;
        gameIsPaused = false;

        // TODO: Enable all buttons behind the pause menu & select something
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

        // TODO: Disable all buttons behind the pause menu
    }

    public void LoadMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Returning to Main Menu!");
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
