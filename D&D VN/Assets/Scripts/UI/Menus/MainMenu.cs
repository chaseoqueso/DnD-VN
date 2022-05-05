using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    [SerializeField] private string startSceneName;

    // Start is called before the first frame update
    void Start()
    {
        playButton.Select();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
