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
    public static CharacterInstance activeCharacter;
    public static CharacterActionData activeAction;

    [SerializeField] private GameObject chargeSliderOverlay;
    public float abilityChargePercent {get; private set;}

    public GameObject enemyPrefab;
    public GameObject enemyUIHolder;
    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();

    public GameObject timelineIconPrefab;
    public GameObject timelineHolder;
    [HideInInspector] public List<TimelineIcon> timeline = new List<TimelineIcon>();

    public void EnableCombatUI(bool set)
    {
        combatUIIsActive = set;
        combatUIPanel.SetActive(set);

        if(set){
            actionButtons[0].Button().Select();
            // TODO: other stuff
        }
    }

    #region Ability Charge and Queue
        public void ToggleAbilityChargeOverlay(bool set)
        {
            chargeSliderOverlay.SetActive(set);
        }

        public void ToggleAbilityChargeOverlay(bool set, EntityID id)
        {
            chargeSliderOverlay.SetActive(set);

            if(set){
                CreatureInstance targetCreature = TurnManager.Instance.GetCharacter(id);
                ChargeAndQueueAbility(targetCreature);
            }
        }

        public void ToggleAbilityChargeOverlay(bool set, int enemyIndex)
        {
            chargeSliderOverlay.SetActive(set);

            if(set){
                CreatureInstance targetCreature = TurnManager.Instance.GetEnemy(enemyIndex);
                ChargeAndQueueAbility(targetCreature);
            }
        }

        private void ChargeAndQueueAbility( CreatureInstance target )
        {
            float chargePercent = 0f;

            // TODO: do the charge thing

            var queuedAction = activeAction.PerformAction( activeCharacter, target, chargePercent );
            float delay = activeAction.CalculateChargeDelay(activeCharacter.data, chargePercent);
            TurnManager.Instance.QueueChargedActionForCurrentTurn(queuedAction, delay);

            // Cleanup
            ClearActiveCharacter();
            ClearActiveAction();
            ToggleAbilityChargeOverlay(false);
            
            TurnManager.Instance.StartNextTurn();
        }
    #endregion

    #region Action Buttons
        public void ActionButtonClicked( ActionButtonType actionType )
        {
            CharacterCombatData combatData = activeCharacter.data;

            activeAction = null;
            switch(actionType){
                case ActionButtonType.basicAttack:
                    activeAction = combatData.BasicAttack;
                    break;
                case ActionButtonType.basicGuard:
                    activeAction = combatData.BasicGuard;
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
                    activeAction = combatData.Action1;
                    break;
                case ActionButtonType.action2:
                    activeAction = combatData.Action2;
                    break;
                case ActionButtonType.action3:
                    activeAction = combatData.Action3;
                    break;
                case ActionButtonType.special1:
                    activeAction = combatData.Special1;
                    break;
                case ActionButtonType.special2:
                    activeAction = combatData.Special2;
                    break;
                case ActionButtonType.special3:
                    activeAction = combatData.Special3;
                    break;
            }

            hoverText.text = "Action selected: " + actionType;

            if(activeAction.Target == TargetType.none){
                return;
            }
            else{
                StartTargetCreatureOnActionSelect(activeAction.Target);
            }
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

        public void ClearActiveAction()
        {
            if(activeAction == null){
                return;
            }

            // If necessary, set UI back to normal???

            activeAction = null;
        }
    #endregion

    #region Character UI Management
        public void AssignActiveCharacter(CharacterInstance character)
        {
            ClearActiveCharacter();
            activeCharacter = character;

            CharacterUIPanel charPanel = GetPanelForCharacterWithID(character.data.CharacterID);
            // TODO: UI feedback that this char is now active
            // TEMP
            SetHoverText("Active character: " + activeCharacter.data.CharacterID);

            foreach(ActionButton ab in actionButtons){
                // TODO: Set action panel values
            }
        }

        public void ClearActiveCharacter()
        {
            if(activeCharacter == null){
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
        private void StartTargetCreatureOnActionSelect(TargetType type)
        {
            targetSelectIsActive = true;
            SetHoverText("Select a target!");
            SetAllActionButtonsInteractable(false);

            if(type == TargetType.enemies){
                SetEnemiesInteractable(true);
            }
            else if(type == TargetType.allies){
                SetCharactersInteractable(true);
            }
            else{
                Debug.LogWarning("Cannot start targeting for action with target type: " + type);
            }
        }

        public void EndTargetCreature()
        {
            SetHoverText("...");

            SetEnemiesInteractable(false);
            SetAllActionButtonsInteractable(true);

            targetSelectIsActive = false;
        }

        public void AllyTargeted(EntityID id)
        {
            EndTargetCreature();
            SetHoverText(id + " targeted!");

            ToggleAbilityChargeOverlay(true, id);
        }

        public void EnemyTargeted(int enemyIndex)
        {
            EndTargetCreature();
            SetHoverText("Enemy " + enemyIndex + " targeted!");

            ToggleAbilityChargeOverlay(true, enemyIndex);
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
        public void SpawnEnemy( int index, Sprite enemyPortrait, Sprite enemyIcon )
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            newEnemy.transform.parent = enemyUIHolder.transform;
            enemies.Add(newEnemy);
            newEnemy.GetComponent<Button>().interactable = targetSelectIsActive;

            newEnemy.GetComponent<EnemyUIPanel>().SetEnemyIndex(index);
            newEnemy.GetComponent<EnemyUIPanel>().SetEnemyPortrait(enemyPortrait);

            // TODO: Add to the timeline with the enemyIcon
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
    #endregion
}
