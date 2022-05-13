using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

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
        }

        if(encounter == null)
        {
            Debug.LogError("No encounter data provided.");
        }
        else
        {
            foreach(EnemyCombatData enemyData in encounter.enemies)
            {
                EnemyInstance enemy = new EnemyInstance(enemyData, enemyData.MaxHP);
                enemyInstances.Add(enemy);

                float initialTurnLength = enemyData.TurnLength;
                if(enemyData.SlowFirstTurn)
                {
                    initialTurnLength *= 2;
                }
                turnOrder.Enqueue(enemy, initialTurnLength);
            }

            AddEnemiesToBattlefield();

            StartNextTurn();

            UIManager.instance.combatUI.EnableCombatUI(true);
        }
    }

    public void StartNextTurn()
    {
        CreatureInstance creature = turnOrder.First;

        if(creature.isChargingAction)
        {
            Debug.Log("Unleashing charged ability from " + creature.data.EntityID.ToString());

            // If the creature has a charged action, perform that action and requeue them in the turn order
            creature.PerformChargedAction();
            RequeueCurrentTurn(creature.data.TurnLength);
            
            // Update the dialog box to display what just happened and disable the action buttons
            var dialogBox = UIManager.instance.combatUI.GetDialogueBox();
            dialogBox.SetDialogueBoxText(creature.GetCurrentActionDescription(), true);
            UIManager.instance.combatUI.SetAllActionButtonsInteractable(false);
            
            // Hook up the progress button to wait for the player to acknowledge what just happened and then continue
            dialogBox.ToggleProgressButton(true, () => 
            {
                UIManager.instance.combatUI.SetAllActionButtonsInteractable(true);
                StartNextTurn();
            });
        }
        else if(creature is CharacterInstance)
        {
            Debug.Log("Starting turn for character " + creature.data.EntityID.ToString());

            if(creature.IsAlive()){
                // If it's a character's turn, hand the torch over to the combat UI
                UIManager.instance.combatUI.AssignActiveCharacter((CharacterInstance)creature);
            }
            else{
                RequeueCurrentTurn(creature.data.TurnLength);
                StartNextTurn();
            }
        }
        else if(creature is EnemyInstance)
        {
            Debug.Log("Starting turn for enemy " + creature.data.EntityID.ToString());

            //If it's the enemy's turn, pick a random action and a random target and perform it instantly
            EnemyInstance enemy = (EnemyInstance)creature;
            ActionData action = enemy.GetNextAction();
            CharacterInstance target = GetCharacter(Random.Range(0, characterInstances.Count));
            enemy.QueueChargedAction(action.PerformAction(enemy, target), action.GetAbilityPerformedDescription(enemy, target));
            RequeueCurrentTurn(0);
            
            // Update the dialog box to display what just happened and disable the action buttons
            var dialogBox = UIManager.instance.combatUI.GetDialogueBox();
            dialogBox.SetDialogueBoxText(creature.GetCurrentActionDescription(), true);
            UIManager.instance.combatUI.SetAllActionButtonsInteractable(false);
            StartNextTurn();
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
            UIManager.instance.combatUI.SpawnEnemy(i, enemyInstances[i].data.Portrait, enemyInstances[i].data.Icon, enemyInstances[i].data.Description, enemyInstances[i].data.MaxHP);
        }
    }

    public void RemoveEnemyFromBattlefield(EnemyInstance enemy)
    {
        int enemyIndex = GetEnemyIndex(enemy);
        if(enemyIndex < 0 || enemyIndex >= enemyInstances.Count){
            return;
        }
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

    public void QueueChargedActionForCurrentTurn(QueuedAction action, string actionDescription, float delay)
    {
        turnOrder.First.QueueChargedAction(action, actionDescription);
        RequeueCurrentTurn(delay);
    }

    public void RequeueCurrentTurn(float delay)
    {
        turnOrder.Enqueue(turnOrder.Dequeue(), currentTurn + delay);
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
        return characterInstances.Find((CharacterInstance c) => c.data.EntityID == entityID);
    }

    public CharacterInstance GetCharacter(CharacterCombatData characterData)
    {
        return characterInstances.Find((CharacterInstance c) => c.data == characterData);
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
}
