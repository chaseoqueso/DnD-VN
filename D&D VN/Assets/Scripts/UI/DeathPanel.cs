using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Fungus;

public class DeathPanel : MonoBehaviour
{
    [SerializeField] private GameObject characterDeathPanel;
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text deathMessage;
    [SerializeField] private TMP_Text buttonMessage;

    public static bool deathPanelIsActive;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == GameManager.COMBAT_DEATH_SCENE){
            characterDeathPanel.SetActive(true);
            deathPanelIsActive = true;

            header.text = "Retreat";
            deathMessage.text = "Carrying an unconscious " + Settings.playerName + " along with them, the party retreated to fight again once their cleric was revived.";
            buttonMessage.text = "Try Again";
        }
    }

    public void SetDeathPanelActive(bool set)
    {
        characterDeathPanel.SetActive(set);
        deathPanelIsActive = set;

        // If not in combat
        if(set && !GameManager.instance.SceneIsCombat()){
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
            Flowchart f = FindObjectOfType<Flowchart>();
            f.SendMessage("Start");
            f.SendFungusMessage("Start");
        }
    }
}
