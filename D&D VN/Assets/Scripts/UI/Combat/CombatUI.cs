using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Priority_Queue;

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
    [SerializeField] private GameObject cancelActionLayoutGroup;

    [SerializeField] private DialogueBox dialogueBox;

    public List<CharacterUIPanel> characterPanels = new List<CharacterUIPanel>();
    public static CharacterInstance activeCharacter;
    public static CharacterActionData activeAction;

    public static CreatureInstance activeTargetCreature;

    [SerializeField] private GameObject abilityChargeOverlay;
    [SerializeField] private Slider chargeSlider;
    public float abilityChargePercent {get; private set;}

    public GameObject enemyPrefab;
    public GameObject enemyUIHolder;
    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();

    public GameObject timelineIconPrefab;
    public GameObject timelineHolder;
    [HideInInspector] public Dictionary<CreatureInstance,TimelineIcon> timelineDatabase = new Dictionary<CreatureInstance,TimelineIcon>();

    private bool specialAbilitySelected = false;

    private const float SLIDER_INCREMENT_VALUE = 0.001f;

    void Start()
    {
        chargeSlider.minValue = 0;
        chargeSlider.maxValue = 1;
    }

    void Update()
    {
        if(abilityChargeIsActive){
            chargeSlider.value += SLIDER_INCREMENT_VALUE;
            if(chargeSlider.value == chargeSlider.maxValue){
                chargeSlider.value = chargeSlider.minValue;
            }
        }
    }

    public void EnableCombatUI(bool set)
    {
        combatUIIsActive = set;
        combatUIPanel.SetActive(set);

        if(set){
            SetAllCharacterPanelValuesOnNewEncounter();
        }
    }

    #region Ability Charge and Queue
        // Min and max x values ON THE SCREEN
        // Charge tags must have anchor set to upper left corner
        private const float CHARGE_X_MIN = 5f;
        private const float CHARGE_X_MAX = 630f;
        private const float CHARGE_Y_POS = -38f;
        
        // If spawning one on top of the other, we want the new yPos to be 50px (the size of the icon) higher
        private const float CHARGE_TAG_Y_POS_OFFSET = 50f;

        [Tooltip("Child of the slider, holds the charge tags as they are spawned on screen")]
        [SerializeField] private GameObject chargeTagHolder;

        // Charge tag prefab
        [SerializeField] private GameObject chargeTagPrefab;

        [SerializeField] private TMP_Text actionMessage;

        [SerializeField] private GameObject chargeKeybindingMessage;
        [SerializeField] private GameObject chargeConfirmCancelButtonHolder;

        public void ToggleAbilityChargeOverlay(bool set)
        {
            abilityChargeOverlay.SetActive(set);

            if(set){
                timelineHolder.transform.SetParent(abilityChargeOverlay.transform, false);
                chargeSlider.value = chargeSlider.minValue;
                SetChargeTags();
                chargeConfirmCancelButtonHolder.GetComponentInChildren<Button>().Select();

                // TODO: action + target?
                actionMessage.text = "[action message]";
            }
            else{
                abilityChargeIsActive = false;
                ToggleAbilityChargeOverlayButtonsActive(true);
                timelineHolder.transform.SetParent(combatUIPanel.transform, false);
                DeleteAllChargeTags();
                foreach( TimelineIcon icon in timelineDatabase.Values ){
                    icon.SetIconNormalColor();
                }
            }
        }

        public void AbilityChargeConfirmButtonClicked()
        {
            abilityChargeIsActive = true;
            ToggleAbilityChargeOverlayButtonsActive(false);
        }

        public void AbilityChargeCancelButtonClicked()
        {
            ToggleAbilityChargeOverlay(false);
            StartTargetCreatureOnActionSelect(activeAction.Target);
        }

        private void ToggleAbilityChargeOverlayButtonsActive(bool set)
        {
            chargeConfirmCancelButtonHolder.SetActive(set);
            chargeKeybindingMessage.SetActive(!set);
        }

        private void SetChargeTags()
        {
            // Get active character's turn length
            float turnLength = activeCharacter.data.TurnLength;

            // Get the current turn
            float currentTurn = TurnManager.Instance.currentTurn;

            // Get tMin and tMax from the action
            float turnMin = currentTurn + activeAction.MinChargeLengthMultiplier * turnLength;
            float turnMax = currentTurn + activeAction.MaxChargeLengthMultiplier * turnLength;

            float yPosOffset = CHARGE_TAG_Y_POS_OFFSET;

            // For all already queued actions that fall within tMin and tMax, instantiate tags
            foreach(CreatureInstance creature in TurnManager.Instance.turnOrder){
                // If creature is null, dead, or ACTIVE, continue (no tag)
                if(creature == null || !creature.IsAlive()){
                    continue;
                }

                // If this is the active character, give UI feedback of that and move on (no tag)
                if(creature == activeCharacter){
                    timelineDatabase[creature].HighlightIcon();
                    continue;
                }

                // For checking if things overlap
                float previousTurnNumber = -1f;

                // If turn number is out of bounds, gray out the timeline icon and move on (no tag)
                float turnNumber = TurnManager.Instance.turnOrder.GetPriority(creature);
                if(turnNumber < turnMin || turnNumber > turnMax){
                    timelineDatabase[creature].GrayOutIcon();
                    continue;
                }

                // Get xPos based on placement between min and max
                float percentFill = Mathf.InverseLerp(turnMin, turnMax, turnNumber);
                float xPosition = Mathf.Lerp(CHARGE_X_MIN, CHARGE_X_MAX, percentFill);

                // Create the tag and set it's position and values
                GameObject tag = Instantiate(chargeTagPrefab, Vector2.zero, Quaternion.identity, chargeTagHolder.transform);
                Vector2 posVector;
                if(turnNumber == previousTurnNumber){
                    posVector = new Vector2(xPosition, CHARGE_Y_POS + yPosOffset);
                    yPosOffset += yPosOffset;
                }
                else{
                    posVector = new Vector2(xPosition, CHARGE_Y_POS);
                    yPosOffset = CHARGE_TAG_Y_POS_OFFSET;
                }
                tag.GetComponent<RectTransform>().anchoredPosition = posVector;
                tag.GetComponent<ChargeTag>().SetValues( creature.data.Icon );

                previousTurnNumber = turnNumber;
            }
            Debug.Log("Number of tags: " + chargeTagHolder.GetComponentsInChildren<ChargeTag>().Length);
        }

        private void DeleteAllChargeTags()
        {
            ChargeTag[] chargeTags = chargeTagHolder.GetComponentsInChildren<ChargeTag>();
            foreach( ChargeTag tag in chargeTags ){
                Destroy(tag.gameObject);
            }
        }

        public void QueueAbilityOnChargeComplete()
        {
            abilityChargeIsActive = false;

            float chargePercent = chargeSlider.value;
            Debug.Log("Charged ability value: " + chargePercent);

            EntityID id = activeCharacter.data.EntityID;

            var queuedAction = activeAction.GetQueuedAction( activeCharacter, activeTargetCreature, chargePercent );
            float delay = activeAction.CalculateChargeDelay(activeCharacter.data, chargePercent);
            TurnManager.Instance.QueueChargedActionForCurrentTurn(queuedAction, delay);
            
            if(specialAbilitySelected)
            {
                GameManager.instance.SpendSkillPointForCharacter(id);
                GetPanelForCharacterWithID(id).SetSkillPointUI(GameManager.instance.GetCurrentSkillPoints(id));
            }

            // Cleanup
            StartCoroutine(CloseAbilityChargeRoutine());
            ClearActiveCharacter();
            ClearActiveAction();            
        }

        public IEnumerator CloseAbilityChargeRoutine()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            ToggleAbilityChargeOverlay(false);
            ResetAllSecondaryActionPanels();
            TurnManager.Instance.StartNextTurn();
        }
    #endregion

    #region Action Buttons
        public void ActionButtonClicked( ActionButtonType actionType )
        {
            CharacterCombatData combatData = activeCharacter.data;

            activeAction = null;
            specialAbilitySelected = false;
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
                    ResetAllSecondaryActionPanels();
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
                    specialAbilitySelected = true;
                    break;
                case ActionButtonType.special2:
                    activeAction = combatData.Special2;
                    specialAbilitySelected = true;
                    break;
                case ActionButtonType.special3:
                    activeAction = combatData.Special3;
                    specialAbilitySelected = true;
                    break;
            }

            dialogueBox.SetDialogueBoxText("Action selected: " + actionType, true);
            ToggleCancelActionPanel(true);

            if(activeAction.Target == TargetType.none){
                return;
            }
            else{
                StartTargetCreatureOnActionSelect(activeAction.Target);
            }
        }

        private void ResetAllSecondaryActionPanels()
        {
            ToggleSecondaryActionPanel(false);
            ToggleSecondarySpecialPanel(false);
            ToggleCancelActionPanel(false);
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

        private void ToggleCancelActionPanel(bool set)
        {
            // If we called this from an action in a sub-menu, make sure those are closed too
            if(set){
                actionSecondaryLayoutGroup.SetActive(false);
                specialSecondaryLayoutGroup.SetActive(false);
            }

            cancelActionLayoutGroup.SetActive(set);
            baseActionLayoutGroup.SetActive(!set);
        }

        public void CancelActiveAction()
        {
            CancelTargetCreature();
            ClearActiveAction();
            ToggleCancelActionPanel(false);

            // TEMP
            dialogueBox.SetDialogueBoxText("Active character: " + activeCharacter.data.EntityID, true);
        }

        private void ClearActiveAction()
        {
            // if(activeAction == null){
            //     return;
            // }
            // update UI if necessary?
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
                if(type == ActionButtonType.actionPanelToggle || type == ActionButtonType.back){
                    continue;
                }
                if(type == ActionButtonType.specialPanelToggle)
                {
                    if(GameManager.instance.GetCurrentSkillPoints(character.data.EntityID) <= 0)
                        ab.Button().interactable = false;

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

                c.SetValues( character.GetCurrentHealth(), GameManager.instance.GetCurrentSkillPoints(character.data.EntityID), character.data.Description, character.data.Icon );
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
            GetPanelForCharacterWithID(id).SetValues( character.GetCurrentHealth(), GameManager.instance.GetCurrentSkillPoints(id) );
        }

        public void UpdateCharacterPanelValuesForCharacterWithID(CharacterInstance character)
        {
            GetPanelForCharacterWithID( character.data.EntityID ).SetValues( character.GetCurrentHealth(), GameManager.instance.GetCurrentSkillPoints(character.data.EntityID) );
        }

        public void UpdateMainCharacterName()
        {
            CharacterUIPanel c = GetPanelForCharacterWithID( EntityID.MainCharacter );
            c.SetCharacterName( Settings.playerName );
        }
    #endregion

    #region Targeting
        private void StartTargetCreatureOnActionSelect(TargetType type)
        {
            dialogueBox.SetDialogueBoxText("Select a target!", true);

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
            SetAlliesInteractable(false);

            enemySelectIsActive = false;
            allySelectIsActive = false;
            activeTargetCreature = null;
        }

        public void AllyTargeted(EntityID id)
        {
            EndTargetCreature();

            activeTargetCreature = TurnManager.Instance.GetCharacter(id);
            ToggleAbilityChargeOverlay(true);
        }

        public void EnemyTargeted(int enemyIndex)
        {
            EndTargetCreature();

            activeTargetCreature = TurnManager.Instance.GetEnemy(enemyIndex);
            ToggleAbilityChargeOverlay(true);
        }
    #endregion

    #region Timeline Management
        public void AddEntityToTimeline( CreatureInstance creature )
        {
            GameObject newIcon = Instantiate(timelineIconPrefab, new Vector3(0,0,0), Quaternion.identity);
            newIcon.transform.SetParent(timelineHolder.transform, false);

            TimelineIcon ti = newIcon.GetComponent<TimelineIcon>();
            ti.SetTimelineIconValues(creature.data.EntityID, creature.data.Icon);
            timelineDatabase.Add(creature, ti);

            UpdateTimelineOrder();
        }

        public void RemoveEntityFromTimeline( CreatureInstance enemy )
        {
            // Destroy the actual UI element in the scene
            Destroy(timelineDatabase[enemy].gameObject);

            // Remove the key from the database
            timelineDatabase.Remove(enemy);

            UpdateTimelineOrder();
        }

        public void UpdateTimelineOrder()
        {
            int index = 0;
            foreach(CreatureInstance creature in TurnManager.Instance.turnOrder){
                if(creature != null && timelineDatabase.ContainsKey(creature)){
                    TimelineIcon icon = timelineDatabase[creature];
                    icon.transform.SetSiblingIndex(index);
                    index++;
                }
            }
        }
    #endregion

    #region Enemy UI Management
        public void SpawnEnemy( int index, Sprite enemyPortrait, Sprite enemyIcon, string description, float health )
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            newEnemy.transform.SetParent(enemyUIHolder.transform, false);
            enemies.Add(newEnemy);
            newEnemy.GetComponent<Button>().interactable = enemySelectIsActive;

            newEnemy.GetComponent<EnemyUIPanel>().SetEnemyPanelValues(index, enemyPortrait, description, health);
        }

        // Called if you inspect an enemy to update their description to the secret description
        public void UpdateEnemyDescriptionWithIndex(int index, string description)
        {
            if( index < 0 || index > enemies.Count ){
                Debug.LogError("Index " + index + " out of bounds for enemy list. Failed to update description");
                return;
            }
            
            if(enemies[index] == null){
                return;
            }

            enemies[index].GetComponent<EnemyUIPanel>().SetEnemyDescription(description);
        }

        public void UpdateEnemyHealth(int index, float health)
        {
            if( index < 0 || index > enemies.Count ){
                Debug.LogError("Index " + index + " out of bounds for enemy list. Failed to update health UI");
                return;
            }

            if(enemies[index] == null){
                return;
            }

            enemies[index].GetComponent<EnemyUIPanel>().UpdateHealthUI(health);
        }

        // When an enemy dies, remove it from scene + list
        public void RemoveEnemyWithID(int index)
        {
            if( index < 0 || index > enemies.Count ){
                Debug.LogError("Index " + index + " out of bounds for enemy list. Failed to remove enemy");
                return;
            }

            if(enemies[index] == null){
                return;
            }

            Destroy(enemies[index].gameObject);
            enemies[index] = null;
        }

        public void SetEnemiesInteractable(bool set)
        {
            if(!enemySelectIsActive){
                return;
            }

            foreach(GameObject e in enemies){
                if(e == null){
                    continue;
                }
                e.GetComponent<Button>().interactable = set;
            }
        }
    #endregion

    public void UpdateUIAfterCreatureHealed(CreatureCombatData data, float currentHP)
    {
        // TEMP - this should be specific to enemies or characters, but for now only MC can heal so this is fine placeholder
        foreach(CharacterUIPanel c in characterPanels){
            if(c.GetCharacterCombatData().EntityID == data.EntityID){
                c.UpdateHealthUI( currentHP );
            }
        }
    }

    public DialogueBox GetDialogueBox()
    {
        return dialogueBox;
    }
}
