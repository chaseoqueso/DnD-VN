using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SpeakerID{
    MainCharacter,
    Sorcerer,
    Knight,
    Monk,
    enumSize
}

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject combatUIPanel;

    [SerializeField] private GameObject actionButtonRow;
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button basicGuardButton;
    [SerializeField] private Button simpleActionButton;
    [SerializeField] private Button primarySpecialSkillButton;
    [SerializeField] private Button secondarySpecialSkillButton;

    [SerializeField] private Button mainCharacterButton;
    [SerializeField] private Button sorcererButton;
    [SerializeField] private Button knightButton;
    [SerializeField] private Button monkButton;

    [SerializeField] private TMP_Text skillPointCount;
    [SerializeField] private TMP_Text activeCharacterText;

    public static SpeakerID activeCharacter;


    void Start()
    {
        activeCharacter = SpeakerID.enumSize;
        activeCharacterText.text = "";
        actionButtonRow.SetActive(false);
    }


    #region Action Buttons
        public void OnBasicAttackClicked()
        {
            Debug.Log("Basic Attack selected");
        }

        public void OnBasicGuardClicked()
        {
            Debug.Log("Basic Guard selected");
        }

        public void OnSimpleActionClicked()
        {
            Debug.Log("Simple Action selected");
        }

        public void OnPrimarySpecialClicked()
        {
            Debug.Log("Primary Special selected");
        }

        public void OnSecondarySpecialClicked()
        {
            Debug.Log("Secondary Special selected");
        }
    #endregion


    #region Character Buttons
        public void MainCharacterClicked()
        {
            activeCharacter = SpeakerID.MainCharacter;
            activeCharacterText.text = "Active Character: Main Character";
            actionButtonRow.SetActive(true);
        }

        public void SorcererClicked()
        {
            activeCharacter = SpeakerID.Sorcerer;
            activeCharacterText.text = "Active Character: Sorcerer";
            actionButtonRow.SetActive(true);
        }

        public void KnightClicked()
        {
            activeCharacter = SpeakerID.Knight;
            activeCharacterText.text = "Active Character: Knight";
            actionButtonRow.SetActive(true);
        }

        public void MonkClicked()
        {
            activeCharacter = SpeakerID.Monk;
            activeCharacterText.text = "Active Character: Monk";
            actionButtonRow.SetActive(true);
        }
    #endregion


    public void SetCurrentSkillPoints(int value)
    {
        skillPointCount.text = "SP: " + value;
    }
}
