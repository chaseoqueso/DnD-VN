using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterUIPanel : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CharacterCombatData characterData;

    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text skillPointLabel;

    private string characterDescription = "[character description]";

    [SerializeField] private List<Image> skillPointSlots = new List<Image>();

    void Start()
    {
        if(!characterData){
            Debug.LogError("No speaker data assigned for a combat character panel!");
            return;
        }
    }

    public EntityID GetCharacterUIPanelID()
    {
        return characterData.EntityID;
    }

    public CharacterCombatData GetCharacterCombatData()
    {
        return characterData;
    }

    public void SetValues(float currentHealthValue, int currentSkillPoints, string description)
    {
        if(characterData.EntityID == EntityID.MainCharacter){
            characterNameText.text = Settings.playerName;
        }
        else{
            characterNameText.text = characterData.EntityID.ToString();
        }
        skillPointLabel.text = characterData.SkillPointName;
        characterDescription = description;
        UpdateHealthUI(currentHealthValue);
        SetSkillPointUI(currentSkillPoints);
    }

    public void SetValues(float currentHealthValue, int currentSkillPoints)
    {
        UpdateHealthUI(currentHealthValue);
        SetSkillPointUI(currentSkillPoints);
    }

    public void UpdateHealthUI(float health)
    {
        healthText.text = Mathf.CeilToInt(health) + "";
    }

    public void SetSkillPointUI(int skillPoints)
    {
        if(skillPoints > skillPointSlots.Count){
            Debug.LogWarning("Cannot set current SP above SP max! Setting to max instead.");
        }
        else if(skillPoints < 0){
            Debug.LogError("Cannot decrease skill points below 0!");
        }

        for(int i = 0; i < skillPointSlots.Count; i++){
            Image img = skillPointSlots[i];
            if(i <= skillPoints - 1){
                // Set it full
                UIManager.SetImageColorFromHex(img, UIManager.GOLD_COLOR);
            }
            else{
                // Set it empty
                UIManager.SetImageColorFromHex(img, UIManager.OFF_WHITE_COLOR);
            }
        }
    }

    public void OnCharacterSelected()
    {
        UIManager.instance.combatUI.AllyTargeted(characterData.EntityID);
    }

    #region Hover Text
        public void OnPointerEnter(PointerEventData eventData)
        {
            HoverAction();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExitAction();
        }

        public void OnSelect(BaseEventData eventData)
        {
            HoverAction();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ExitAction();
        }

        private void HoverAction()
        {
            if(!CombatUI.allySelectIsActive){
                return;
            }

            UIManager.instance.combatUI.SetHoverText(characterDescription);
        }

        private void ExitAction()
        {
            if(!CombatUI.allySelectIsActive){
                return;
            }
            
            UIManager.instance.combatUI.SetHoverText("");
        }
    #endregion
}
