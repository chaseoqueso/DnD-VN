using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Color OFF_WHITE_COLOR; // = "d7d9d3";
    [SerializeField] private Color LIGHT_BROWN_COLOR; // = "6b604f";
    [SerializeField] private Color MED_BROWN_COLOR; // = "423c39";
    [SerializeField] private Color DARK_BROWN_COLOR; // = "1f1b1b";
    [SerializeField] private Color BLUE_COLOR; // = "6291bd";
    [SerializeField] private Color GOLD_COLOR; // = "bdaa61";
    [SerializeField] private Color RED_COLOR; // = "bd7465";

    [SerializeField] private Button pauseButton;

    public CombatUI combatUI;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    #region Colors
        public Color OffWhiteColor()
        {
            return OFF_WHITE_COLOR;
        }

        public Color LightBrownColor()
        {
            return LIGHT_BROWN_COLOR;
        }

        public Color MedBrownColor()
        {
            return MED_BROWN_COLOR;
        }

        public Color DarkBrownColor()
        {
            return DARK_BROWN_COLOR;
        }

        public Color BlueColor()
        {
            return BLUE_COLOR;
        }

        public Color GoldColor()
        {
            return GOLD_COLOR;
        }

        public Color RedColor()
        {
            return RED_COLOR;
        }
    #endregion

    public void SetButtonsInteractable(bool set)
    {
        pauseButton.interactable = set;

        if(CombatUI.combatUIIsActive){
            combatUI.SetAllActionButtonsInteractable(set);
            combatUI.SetEnemiesInteractable(set);
        }

        if(set){
            // TODO: Select a button
        }
    }
}
