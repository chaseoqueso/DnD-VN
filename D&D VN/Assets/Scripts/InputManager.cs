using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnPause(InputValue input)
    {
        if(!PauseMenu.instance.gameIsPaused){
            PauseMenu.instance.PauseGame();
        }
        else{
            PauseMenu.instance.ResumeGame();
        }
    }
}
