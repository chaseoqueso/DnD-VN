using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUIPanel : MonoBehaviour
{
    [SerializeField] private SpeakerData speakerData;

    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text skillPointLabel;

    [SerializeField] private List<Image> skillPointSlots;
    [HideInInspector] public int currentSkillPoints;

    void Start()
    {
        currentSkillPoints = skillPointSlots.Count;

        if(!speakerData){
            Debug.LogError("No speaker data assigned for a combat character panel!");
            return;
        }

        // SetValues(speakerData.MaxHealth());
    }

    public EntityID GetCharacterUIPanelID()
    {
        return speakerData.EntityID();
    }

    public void SetValues(int currentHealthValue)
    {
        if(speakerData.EntityID() == EntityID.MainCharacter){
            characterNameText.text = Settings.playerName;
        }
        else{
            characterNameText.text = speakerData.EntityID().ToString();
        }
        // skillPointLabel.text = speakerData.SkillPointName();
        UpdateHealthUI(currentHealthValue);
    }

    public void UpdateHealthUI(int health)
    {
        healthText.text = health + "";
    }

    public void DecreaseSkillPointUI()
    {
        if(currentSkillPoints == 0){
            Debug.LogWarning("Cannot decrease skill points below 0! Should not have called DecreaseSkillPointUI while at 0!");
            return;
        }
        currentSkillPoints--;

        // skillPointSlots[currentSkillPoints].color = ;
    }
}
