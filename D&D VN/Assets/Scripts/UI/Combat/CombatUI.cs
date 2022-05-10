using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject combatUIPanel;
    public static bool combatUIIsActive = false;
    public static bool enemySelectIsActive = false;
    public static bool allySelectIsActive = false;
    public static bool abilityChargeIsActive = false;

    [Tooltip("Base action panel object; the thing you would disable if you wanted the ENTIRE thing gone.")]
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private List<ActionButton> actionButtons = new List<ActionButton>();

    [SerializeField] private GameObject baseActionLayoutGroup;
    [SerializeField] private GameObject actionSecondaryLayoutGroup;
    [SerializeField] private GameObject specialSecondaryLayoutGroup;

    [SerializeField] private DialogueBox dialogueBox;

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
            SetAllCharacterPanelValuesOnNewEncounter();
        }
    }

    #region Ability Charge and Queue
        public void ToggleAbilityChargeOverlay(bool set)
        {
            chargeSliderOverlay.SetActive(set);
            abilityChargeIsActive = set;
        }

        public void ToggleAbilityChargeOverlay(bool set, EntityID id)
        {
            ToggleAbilityChargeOverlay(set);

            if(set){
                CreatureInstance targetCreature = TurnManager.Instance.GetCharacter(id);
                ChargeAndQueueAbility(targetCreature);
            }
        }

        public void ToggleAbilityChargeOverlay(bool set, int enemyIndex)
        {
            ToggleAbilityChargeOverlay(set);

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
            string description = activeAction.GetAbilityPerformedDescription(activeCharacter, target, chargePercent);
            TurnManager.Instance.QueueChargedActionForCurrentTurn(queuedAction, description, delay);

            // Cleanup
            ClearActiveCharacter();
            ClearActiveAction();
            ToggleAbilityChargeOverlay(false);
            ResetBothSecondaryActionPanels();

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
                    ResetBothSecondaryActionPanels();
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

            dialogueBox.SetDialogueBoxText("Action selected: " + actionType, true);

            if(activeAction.Target == TargetType.none){
                return;
            }
            else{
                StartTargetCreatureOnActionSelect(activeAction.Target);
            }
        }

        private void ResetBothSecondaryActionPanels()
        {
            ToggleSecondaryActionPanel(false);
            ToggleSecondarySpecialPanel(false);
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

        public void SetAllActionButtonsInteractable(bool set)
        {
            foreach(ActionButton b in actionButtons){
                b.Button().interactable = set;
            }
        }
    #endregion

    #region Character UI Management
        public void AssignActiveCharacter(CharacterInstance character)
        {
            ClearActiveCharacter();
            activeCharacter = character;

            CharacterUIPanel charPanel = GetPanelForCharacterWithID(character.data.EntityID);
            UIManager.SetImageColorFromHex( charPanel.GetComponent<Image>(), UIManager.BLUE_COLOR );

            // TEMP
            dialogueBox.SetDialogueBoxText("Active character: " + activeCharacter.data.EntityID, true);

            foreach(ActionButton ab in actionButtons){
                ActionButtonType type = ab.ActionType();
                if(type == ActionButtonType.actionPanelToggle || type == ActionButtonType.specialPanelToggle || type == ActionButtonType.back){
                    continue;
                }
                CharacterActionData actionData = character.data.GetActionFromButtonType(type);
                ab.SetActionValues(actionData);
            }

            actionButtons[0].Button().Select();
        }

        public void ClearActiveCharacter()
        {
            if(activeCharacter == null){
                return;
            }

            UIManager.SetImageColorFromHex( GetPanelForCharacterWithID(activeCharacter.data.EntityID).GetComponent<Image>(), UIManager.MED_BROWN_COLOR );

            activeCharacter = null;
        }

        public void SetAlliesInteractable(bool set)
        {
            if(!allySelectIsActive){
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

        public void SetAllCharacterPanelValuesOnNewEncounter()
        {
            foreach( CharacterUIPanel c in characterPanels ){
                CharacterInstance character = TurnManager.Instance.GetCharacter( c.GetCharacterUIPanelID() );

                c.SetValues( character.GetCurrentHealth(), character.GetCurrentSkillPoints(), character.data.Description );
            }
        }

        public void UpdateCharacterPanelValuesForCharacterWithID(EntityID id, int currentHealth, int currentSkillPoints)
        {
            CharacterUIPanel c = GetPanelForCharacterWithID(id);
            c.UpdateHealthUI(currentHealth);
            c.SetSkillPointUI(currentSkillPoints);
        }

        public void UpdateCharacterPanelValuesForCharacterWithID(EntityID id)
        {
            CharacterInstance character = TurnManager.Instance.GetCharacter(id);
            GetPanelForCharacterWithID(id).SetValues( character.GetCurrentHealth(), character.GetCurrentSkillPoints() );
        }

        public void UpdateCharacterPanelValuesForCharacterWithID(CharacterInstance character)
        {
            GetPanelForCharacterWithID( character.data.EntityID ).SetValues( character.GetCurrentHealth(), character.GetCurrentSkillPoints() );
        }
    #endregion

    #region Targeting
        private void StartTargetCreatureOnActionSelect(TargetType type)
        {
            dialogueBox.SetDialogueBoxText("Select a target!", true);
            SetAllActionButtonsInteractable(false);

            if(type == TargetType.enemies){
                enemySelectIsActive = true;
                SetEnemiesInteractable(true);
            }
            else if(type == TargetType.allies){
                allySelectIsActive = true;
                SetAlliesInteractable(true);
            }
            else{
                Debug.LogWarning("Cannot start targeting for action with target type: " + type);
            }
        }

        public void CancelTargetCreature()
        {
            // TODO
            EndTargetCreature();
        }

        public void EndTargetCreature()
        {
            SetEnemiesInteractable(false);
            SetAllActionButtonsInteractable(true);

            enemySelectIsActive = false;
            allySelectIsActive = false;
        }

        public void AllyTargeted(EntityID id)
        {
            EndTargetCreature();
            // dialogueBox.SetDialogueBoxText(id + " targeted!", true);

            ToggleAbilityChargeOverlay(true, id);
        }

        public void EnemyTargeted(int enemyIndex)
        {
            EndTargetCreature();
            // dialogueBox.SetDialogueBoxText("Enemy " + enemyIndex + " targeted!", true);

            ToggleAbilityChargeOverlay(true, enemyIndex);
        }
    #endregion

    #region Timeline Management


        public void AddEntityToTimeline( EntityID id, Sprite iconSprite, int turn )
        {
            GameObject newIcon = Instantiate(timelineIconPrefab, new Vector3(0,0,0), Quaternion.identity);
            newIcon.transform.SetParent(timelineHolder.transform, false);
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
        public void SpawnEnemy( int index, Sprite enemyPortrait, Sprite enemyIcon, string description )
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            newEnemy.transform.SetParent(enemyUIHolder.transform, false);
            enemies.Add(newEnemy);
            newEnemy.GetComponent<Button>().interactable = enemySelectIsActive;

            newEnemy.GetComponent<EnemyUIPanel>().SetEnemyPanelValues(index, enemyPortrait, description);

            // TODO: Add to the timeline with the enemyIcon
        }

        // Called if you inspect an enemy to update their description to the secret description
        public void UpdateEnemyDescriptionWithIndex(int index, string description)
        {
            enemies[index].GetComponent<EnemyUIPanel>().SetEnemyDescription(description);
        }

        // When an enemy dies, remove it from scene + list
        public void RemoveEnemyWithID(int index)
        {
            Destroy(enemies[index].gameObject);
            enemies[index] = null;
        }

        public void SetEnemiesInteractable(bool set)
        {
            if(!enemySelectIsActive){
                return;
            }

            foreach(GameObject e in enemies){
                e.GetComponent<Button>().interactable = set;
            }
        }
    #endregion

    public DialogueBox GetDialogueBox()
    {
        return dialogueBox;
    }
}
