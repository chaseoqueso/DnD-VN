using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnPause(InputValue input)
    {
        if(CombatUI.targetSelectIsActive){
            UIManager.instance.combatUI.EndTargetCreature();
            return;
        }

        if(!PauseMenu.instance.gameIsPaused){
            PauseMenu.instance.PauseGame();
        }
        else{
            PauseMenu.instance.ResumeGame();
        }
    }
}
