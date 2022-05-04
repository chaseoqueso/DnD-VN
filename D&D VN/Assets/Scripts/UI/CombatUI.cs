using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SpeakerID{
    MainCharacter,
    Aeris,
    Samara,
    enumSize
}

// public enum EnemyID{
//     LightMinion,
//     DarkMinion,
//     ArcanaMinion,
//     DragonBoss,
//     enumSize
// }

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

    [SerializeField] private TMP_Text skillPointCount;
    [SerializeField] private TMP_Text activeCharacterText;

    public static SpeakerID activeCharacter;

    public GameObject enemyPrefab;
    public GameObject enemyUIHolder;
    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        activeCharacter = SpeakerID.enumSize;
        activeCharacterText.text = "";
        actionButtonRow.SetActive(false);
        mainCharacterButton.Select();
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
            activeCharacter = SpeakerID.Aeris;
            activeCharacterText.text = "Active Character: Aeris";
            actionButtonRow.SetActive(true);
        }

        public void KnightClicked()
        {
            activeCharacter = SpeakerID.Samara;
            activeCharacterText.text = "Active Character: Samara";
            actionButtonRow.SetActive(true);
        }
    #endregion


    #region Enemy UI Management
        // TODO: Pass in a type? or a sprite? or the entire enemy itself?
        public void SpawnEnemyOfType()
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            newEnemy.transform.parent = enemyUIHolder.transform;
            
            enemies.Add(newEnemy);
        }

    #endregion


    public void SetCurrentSkillPoints(int value)
    {
        skillPointCount.text = "SP: " + value;
    }
}
