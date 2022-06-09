using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathPanel : MonoBehaviour
{
    [SerializeField] private GameObject characterDeathPanel;
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text deathMessage;
    [SerializeField] private TMP_Text buttonMessage;

    public void SetDeathPanelActive(bool set)
    {
        characterDeathPanel.SetActive(set);

        // If in combat
        if(set && GameManager.instance.SceneIsCombat()){
            header.text = "Retreat";
            deathMessage.text = "Carrying an unconscious " + Settings.playerName + " along with them, the party retreated to fight again once their cleric was revived.";
            buttonMessage.text = "Try Again";
        }
        else if(set){
            header.text = "Short Rest";
            deathMessage.text = "The party took a short rest, trying to regain their strength after combat, until...";
            buttonMessage.text = "Continue";
        }
    }

    public void ContinueButtonClicked()
    {
        // If in combat
        if(GameManager.instance.SceneIsCombat()){
            GameManager.instance.RestartCurrentCombat();
        }
        else{
            SetDeathPanelActive(false);
            // TODO: Start dialogue
        }
    }
}
