using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    #region Color Palette Hex Codes
        public const string OFF_WHITE_COLOR = "#d7d9d3";
        public const string LIGHT_BROWN_COLOR = "#6b604f";
        public const string MED_BROWN_COLOR = "#423c39";
        public const string DARK_BROWN_COLOR = "#1f1b1b";
        public const string BLUE_COLOR = "#6291bd";
        public const string GOLD_COLOR = "#bdaa61";
        public const string RED_COLOR = "#bd7465";
    #endregion

    [SerializeField] private Button pauseButton;

    public CombatUI combatUI;

    public GameObject dialogueChoicePanel;
    public GameObject routeSelectPanel;

    public DeathPanel deathPanel;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    public void SetButtonsInteractable(bool set)
    {
        pauseButton.interactable = set;

        if(combatUI && CombatUI.combatUIIsActive){
            combatUI.SetAllActionButtonsInteractable(set);
            combatUI.SetEnemiesInteractable(set);
        }

        if(set){
            // TODO: Select a button
        }
    }

    public void ToggleDialogueChoicePanel(bool set)
    {
        if(dialogueChoicePanel){
            dialogueChoicePanel.SetActive(set);
        }        
    }

    public void ToggleRouteSelectPanel(bool set)
    {
        if(routeSelectPanel){
            routeSelectPanel.SetActive(set);
        }        
    }

    public void ToggleCharacterDeathPanel(bool set)
    {
        if(GameManager.instance.SceneIsCombat()){
            Debug.LogWarning("Should use 'ToggleMainCharacterDeathPanelInCombat' while in combat!");
            return;
        }
        deathPanel.SetDeathPanelActive(set);
    }

    public void ToggleMainCharacterDeathPanelInCombat(bool set)
    {
        if(!GameManager.instance.SceneIsCombat()){
            Debug.LogWarning("Cannot set main character death panel active while not in combat!");
            return;
        }

        SceneManager.LoadScene(GameManager.COMBAT_DEATH_SCENE);
    }

    public void UseSpellSlotOutsideCombat()
    {
        GameManager.instance.SpendSkillPointForCharacter(EntityID.MainCharacter);
    }
    
    public static void SetImageColorFromHex( Image img, string hexCode )
    {
        Color color;
        if(ColorUtility.TryParseHtmlString( hexCode, out color )){
            img.color = color;
        }
        else{
            Debug.LogError("Failed to set color");
        }   
    }
}
