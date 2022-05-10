using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnPause(InputValue input)
    {
        if(CombatUI.enemySelectIsActive || CombatUI.allySelectIsActive){
            UIManager.instance.combatUI.CancelTargetCreature();
            return;
        }

        if(!PauseMenu.instance.gameIsPaused){
            PauseMenu.instance.PauseGame();
        }
        else{
            PauseMenu.instance.ResumeGame();
        }
    }

    public void OnProgress(InputValue input)
    {
        if(CombatUI.combatUIIsActive && DialogueBox.progressButtonIsActive){
            UIManager.instance.combatUI.GetDialogueBox().OnButtonClicked();
        }
    }

    public void OnAbilityCharge(InputValue input)
    {
        if(!CombatUI.abilityChargeIsActive){
            return;
        }

        CombatUI.abilityChargeIsActive = false;
    }
}
