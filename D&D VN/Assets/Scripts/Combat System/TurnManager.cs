using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEngine.Events;

[System.Serializable]
public enum DamageType
{
    Neutral,
    Light,
    Dark,
    Arcane
}

public struct DamageData
{
    public float damageAmount;
    public DamageType damageType;

    public DamageData(float amount, DamageType type)
    {
        damageAmount = amount;
        damageType = type;
    }
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance
    {
        get { return instance; }

        set 
        {
            if(instance == null)
            {
                instance = value;
            }
            else
            {
                Destroy(value.gameObject);
            }
        }
    }
    private static TurnManager instance;

    [Tooltip("The list of data for all characters to be included in this combat.")]
    public List<CharacterCombatData> characterDatas;
    
    [Tooltip("The encounter object for the enemies to be used for this combat.")]
    public EncounterData encounter;

    public float currentTurn { get {return turnOrder.GetPriority(turnOrder.First);} }

    public SimplePriorityQueue<CreatureInstance, float> turnOrder { get; private set; }
    private List<CharacterInstance> characterInstances;
    private List<EnemyInstance> enemyInstances;
    private List<GameObject> enemySprites;

    void Awake()
    {
        Instance = this;

        turnOrder = new SimplePriorityQueue<CreatureInstance, float>();
        characterInstances = new List<CharacterInstance>();
        enemyInstances = new List<EnemyInstance>();
        enemySprites = new List<GameObject>();
    }

    void Start()
    {
        foreach(CharacterCombatData data in characterDatas)
        {
            CharacterInstance character = new CharacterInstance(data, data.MaxHP);
            characterInstances.Add(character);
            turnOrder.Enqueue(character, data.TurnLength);
            UIManager.instance.combatUI.AddEntityToTimeline( character );
        }

        if(encounter == null)
        {
            Debug.LogError("No encounter data provided.");
        }
        else
        {
            foreach(EnemyCombatData enemyData in encounter.enemies)
            {
                EnemyInstance enemy;
                if(enemyData is BossEnemyCombatData)
                {
                    enemy = new BossEnemyInstance(enemyData, enemyData.MaxHP);
                }
                else
                {
                    enemy = new EnemyInstance(enemyData, enemyData.MaxHP);
                }

                enemyInstances.Add(enemy);

                float initialTurnLength = enemyData.TurnLength;
                if(enemyData.SlowFirstTurn)
                {
                    initialTurnLength *= 2;
                }
                turnOrder.Enqueue(enemy, initialTurnLength);
                UIManager.instance.combatUI.AddEntityToTimeline( enemy );
            }

            AddEnemiesToBattlefield();

            StartNextTurn();

            UIManager.instance.combatUI.EnableCombatUI(true);
        }
    }

    public void StartNextTurn()
    {
        UIManager.instance.combatUI.UpdateTimelineOrder();
        CreatureInstance creature = turnOrder.First;

        if(creature.isChargingAction)
        {
            // Debug.Log("Unleashing charged ability from " + creature.data.EntityID.ToString());
            
            // Sisable the action buttons
            UIManager.instance.combatUI.SetAllActionButtonsInteractable(false);

            // If the character gets delayed after their attack, requeue them. Otherwise they take their turn immediately
            if(creature.GetQueuedActionData().DelayAfterActionPerformed)
                RequeueCurrentTurn(creature.GetTurnLength());

            // Perform the charged action
            QueuedAction action = creature.PerformChargedAction();

            // Update the dialog box to display what just happened
            var dialogBox = UIManager.instance.combatUI.GetDialogueBox();
            if(action is ChargeableQueuedAction)
            {
                ChargeableQueuedAction chargeAction = (ChargeableQueuedAction)action;
                dialogBox.SetDialogueBoxText(chargeAction.data.GetAbilityPerformedDescription(chargeAction.source, chargeAction.target, chargeAction.chargePercent), true);
            }
            else
            {
                dialogBox.SetDialogueBoxText(action.data.GetAbilityPerformedDescription(action.source, action.target), true);
            }
            
            // Hook up the progress button to wait for the player to acknowledge what just happened and then continue
            dialogBox.ToggleProgressButton(true, () => 
            {
                UIManager.instance.combatUI.SetAllActionButtonsInteractable(true);
                if(CombatUI.ActiveCharacterIsEnemy() || CombatUI.activeCharacter == null){
                    StartNextTurn();
                }
            });
        }
        else if(creature is CharacterInstance)
        {
            // Debug.Log("Starting turn for character " + creature.data.EntityID.ToString());

            if(creature.IsAlive()){
                // If it's a character's turn, hand the torch over to the combat UI
                creature.StartTurn();
            }
            else{
                RequeueCurrentTurn(creature.GetTurnLength());
                StartNextTurn();
            }
        }
        else if(creature is EnemyInstance)
        {
            // Debug.Log("Starting turn for enemy " + creature.data.EntityID.ToString());
            creature.StartTurn();
            StartNextTurn();
            // UIManager.instance.combatUI.UpdateTimelineOrder();   // Does this go here?
        }
        else
        {
            Debug.LogError("turnOrder contained a value that was neither CharacterInstance nor EnemyInstance.");
        }
    }

    public void AddEnemiesToBattlefield()
    {
        int numPoints = enemyInstances.Count + 1;
        for(int i = 0; i < enemyInstances.Count; ++i)
        {
            UIManager.instance.combatUI.SpawnEnemy(i, enemyInstances[i].GetPortrait(), enemyInstances[i].GetIcon(), enemyInstances[i].GetDescription(), enemyInstances[i].GetMaxHP());
        }
    }

    public void RemoveEnemyFromBattlefield(EnemyInstance enemy)
    {
        int enemyIndex = GetEnemyIndex(enemy);
        if(enemyIndex < 0 || enemyIndex >= enemyInstances.Count){
            return;
        }
        UIManager.instance.combatUI.RemoveEntityFromTimeline(enemy);
        enemyInstances[enemyIndex] = null;
        UIManager.instance.combatUI.RemoveEnemyWithID(enemyIndex);

        // Checks if there is any enemy that is not null
        foreach( EnemyInstance e in enemyInstances ){
            if(e != null){
                return;
            }
        }
        GameManager.instance.EndCombat();
    }

    public void QueueChargedActionForCurrentTurn(QueuedAction action, float delay)
    {
        turnOrder.First.QueueChargedAction(action);
        RequeueCurrentTurn(delay);
    }

    public void RequeueCurrentTurn(float delay)
    {
        RequeueCreature(turnOrder.First, delay);
        // UIManager.instance.combatUI.UpdateTimelineOrder();
    }

    public void RequeueCreature(CreatureInstance creature, float delay)
    {
        turnOrder.UpdatePriority(creature, currentTurn + delay);
    }

    public List<EnemyInstance> GetAllEnemies()
    {
        return new List<EnemyInstance>(enemyInstances);
    }

    public EnemyInstance GetEnemy(int index)
    {
        if(index < 0 || index > enemyInstances.Count)
        {
            Debug.LogError("Index out of bounds.");
        }

        EnemyInstance enemy = enemyInstances[index];

        if(enemy == null)
        {
            Debug.LogWarning("Accessing a null enemy.");
        }

        return enemy;
    }

    public int GetEnemyIndex(EnemyInstance enemy)
    {
        return enemyInstances.IndexOf(enemy);
    }

    public CharacterInstance GetCharacter(EntityID entityID)
    {
        return characterInstances.Find((CharacterInstance c) => c.GetEntityID() == entityID);
    }

    public CharacterInstance GetCharacter(int index)
    {
        if(index < 0 || index > characterInstances.Count)
        {
            Debug.LogError("Index out of bounds.");
        }

        CharacterInstance character = characterInstances[index];

        if(character == null)
        {
            Debug.LogWarning("Accessing a null character.");
        }

        return character;
    }

    public int GetCharacterIndex(CharacterInstance character)
    {
        return characterInstances.IndexOf(character);
    }

    public SortedList<float, List<CreatureInstance>> GetCreaturesInOrder()
    {
        SortedList<float, List<CreatureInstance>> list = new SortedList<float, List<CreatureInstance>>();
        foreach(CreatureInstance creature in turnOrder)
        {
            float turn = turnOrder.GetPriority(creature);
            if(!list.ContainsKey(turn))
                list.Add(turn, new List<CreatureInstance>());

            list[turn].Add(creature);
        }
        return list;
    }
}
