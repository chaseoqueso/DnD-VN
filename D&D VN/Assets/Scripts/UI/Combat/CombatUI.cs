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
        public void ToggleAbilityChargeOverlay(bool set)
        {
            abilityChargeOverlay.SetActive(set);
            abilityChargeIsActive = set;
            chargeSlider.value = chargeSlider.minValue;
        }

        public void QueueAbilityOnChargeComplete()
        {
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
            // EndTargetCreature();
        }

        public void EndTargetCreature()
        {
            SetEnemiesInteractable(false);
            SetAlliesInteractable(false);
            SetAllActionButtonsInteractable(true);

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

        // INCLUSIVE min and max
        public void ToggleGrayOutTimelineEntitiesByTurnRange( int minTurn, int maxTurn, bool set )
        {
            // foreach(TimelineIcon icon in timelineDatabase.Values){
            //     if(icon.turnTriggered >= minTurn && icon.turnTriggered <= maxTurn){
            //         if(set){
            //             icon.GrayOutIcon();
            //         }
            //         else{
            //             icon.SetIconNormalColor();
            //         }
            //     }
            // }
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
