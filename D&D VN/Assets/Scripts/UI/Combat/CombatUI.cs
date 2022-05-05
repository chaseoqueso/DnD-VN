using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject combatUIPanel;
    public static bool combatUIIsActive = false;
    public static bool targetSelectIsActive = false;

    [Tooltip("Base action panel object; the thing you would disable if you wanted the ENTIRE thing gone.")]
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private List<ActionButton> actionButtons = new List<ActionButton>();

    [SerializeField] private GameObject baseActionLayoutGroup;
    [SerializeField] private GameObject actionSecondaryLayoutGroup;
    [SerializeField] private GameObject specialSecondaryLayoutGroup;

    [SerializeField] private TMP_Text hoverText;

    public List<CharacterUIPanel> characterPanels = new List<CharacterUIPanel>();
    public static CharacterCombatData activeCharacter;  // TODO: Change to combat data once Chase pushes that

    [SerializeField] private GameObject chargeSliderOverlay;
    public float abilityChargePercent {get; private set;}

    public GameObject enemyPrefab;
    public GameObject enemyUIHolder;
    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();

    public GameObject timelineIconPrefab;
    public GameObject timelineHolder;
    [HideInInspector] public List<TimelineIcon> timeline = new List<TimelineIcon>();

    void Start()
    {
        // TEMP
        SpawnEnemyOfType();
        

        // TODO: Loop through players + enemies and add them to the timeline based on speed
    }

    public void EnableCombatUI(bool set)
    {
        combatUIIsActive = set;
        combatUIPanel.SetActive(set);

        if(set){
            actionButtons[0].Button().Select();
            // TODO: other stuff
        }
    }

    public void EnableAbilityChargeOverlay(bool set)
    {
        chargeSliderOverlay.SetActive(set);
    }

    #region Action Buttons
        public void ActionButtonClicked( ActionButtonType actionType )
        {
            Debug.Log(actionType + " selected");
            
            switch(actionType){
                case ActionButtonType.basicAttack:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.basicGuard:
                    // TODO: Do the thing
                    return;     // Guard doesn't pick a target so just return
                case ActionButtonType.actionPanelToggle:
                    ToggleSecondaryActionPanel(true);
                    return;     // Toggle the UI and be done
                case ActionButtonType.specialPanelToggle:
                    ToggleSecondarySpecialPanel(true);
                    return;     // Toggle the UI and be done
                case ActionButtonType.back:
                    ToggleSecondaryActionPanel(false);
                    ToggleSecondarySpecialPanel(false);
                    return;     // Toggle the UI and be done
                case ActionButtonType.action1:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.action2:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.action3:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.special1:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.special2:
                    // TODO: Do the thing
                    break;
                case ActionButtonType.special3:
                    // TODO: Do the thing
                    break;
            }

            // if targetable:
            StartTargetCreatureOnActionSelect();
        }

        private void ToggleSecondaryActionPanel(bool set)
        {
            actionSecondaryLayoutGroup.SetActive(set);
            baseActionLayoutGroup.SetActive(!set);
        }
        
        private void ToggleSecondarySpecialPanel(bool set)
        {
            specialSecondaryLayoutGroup.SetActive(set);
            baseActionLayoutGroup.SetActive(!set);
        }
    #endregion

    #region Character UI Management
        public void AssignActiveCharacter(CharacterCombatData characterData)
        {
            ClearActiveCharacter();
            activeCharacter = characterData;

            CharacterUIPanel charPanel = GetPanelForCharacterWithID(characterData.CharacterID);
            // TODO: UI feedback that this char is now active

            foreach(ActionButton ab in actionButtons){
                // TODO: Set action panel values
            }
        }

        public void ClearActiveCharacter()
        {
            if(!activeCharacter){
                return;
            }

            // TODO: Set this panel back to normal
            // GetPanelForCharacterWithID(activeCharacter.CharacterID);

            activeCharacter = null;
        }

        public void SetCharactersInteractable(bool set)
        {
            if(!targetSelectIsActive){
                return;
            }
            foreach(CharacterUIPanel c in characterPanels){
                c.GetComponent<Button>().interactable = set;
            }
        }

        public CharacterUIPanel GetPanelForCharacterWithID(EntityID id)
        {
            foreach(CharacterUIPanel c in characterPanels){
                if(c.GetCharacterUIPanelID() == id){
                    return c;
                }
            }
            Debug.LogError("No character panel found for ID: " + id);
            return null;
        }
    #endregion

    public void SetAllActionButtonsInteractable(bool set)
    {
        foreach(ActionButton b in actionButtons){
            b.Button().interactable = set;
        }
    }

    public void SetHoverText(string text)
    {
        hoverText.text = text;
    }

    #region Targeting
        private void StartTargetCreatureOnActionSelect()
        {
            targetSelectIsActive = true;

            SetHoverText("Select a target!");
            
            // TODO: wait until something is clicked, then do stuff

            SetEnemiesInteractable(true);
            SetAllActionButtonsInteractable(false);
        }

        public void EndTargetCreature()
        {
            SetHoverText("...");

            SetEnemiesInteractable(false);
            SetAllActionButtonsInteractable(true);

            // TODO

            targetSelectIsActive = false;
        }
    #endregion

    #region Timeline Management
        public void AddEntityToTimeline( EntityID id, Sprite iconSprite, int turn )
        {
            GameObject newIcon = Instantiate(timelineIconPrefab, new Vector3(0,0,0), Quaternion.identity);
            newIcon.transform.parent = timelineHolder.transform;
            newIcon.GetComponent<TimelineIcon>().SetTimelineIconValues(id, iconSprite, turn);

            // timeline.Add(newIcon, turn);

            // foreach( GameObject icon in timeline.Keys ){
            //     // If the turn of any icon is greater than THIS action's turn, move this icon in the hierarchy
            //     if( timeline[icon] > turn ){
            //         int index = icon.transform.GetSiblingIndex();
            //         newIcon.transform.SetSiblingIndex(index);
            //     }
            // }
        }
    #endregion

    #region Enemy UI Management
        // TODO: Pass in a type? or a sprite? or the entire enemy itself?
        public void SpawnEnemyOfType()
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            newEnemy.transform.parent = enemyUIHolder.transform;
            enemies.Add(newEnemy);
            UpdateEnemyIndices();
            newEnemy.GetComponent<Button>().interactable = targetSelectIsActive;

            // TODO: Add to the timeline as well!
        }

        private void UpdateEnemyIndices()
        {
            for(int i = 0; i < enemies.Count; i++){
                enemies[i].GetComponent<EnemyUIPanel>().SetEnemyIndex(i);
            }
        }

        public void SetEnemiesInteractable(bool set)
        {
            if(!targetSelectIsActive){
                return;
            }

            foreach(GameObject e in enemies){
                e.GetComponent<Button>().interactable = set;
            }
        }

        public void EnemyTargeted(int enemyIndex)
        {
            EndTargetCreature();
            SetHoverText("Enemy " + enemyIndex + " targeted!");
        }
    #endregion
}
