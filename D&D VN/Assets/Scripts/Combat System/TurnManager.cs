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

public abstract class CreatureInstance
{
    public CreatureCombatData data 
    { 
        get { return _data; }
        protected set { _data = value; }
    }

    public bool isChargingAction { get; protected set; }
    protected QueuedAction queuedAction;

    protected CreatureCombatData _data;
    protected float currentHP;

    public bool IsAlive()
    {
        return currentHP > 0;
    }

    public virtual bool DealDamage(DamageData damage)
    {
        currentHP -= damage.damageAmount * (_data.Defense/100);
        if(currentHP < 0)
        {
            currentHP = 0;
        }

        return IsAlive();
    }

    public void QueueChargedAction(QueuedAction action)
    {
        isChargingAction = true;
        queuedAction = action;
    }

    public void PerformChargedAction()
    {
        queuedAction.Invoke();
    }
}

public class CharacterInstance : CreatureInstance
{
    public new CharacterCombatData data
    {
        get { return (CharacterCombatData) _data; }
        protected set { _data = value; }
    }

    public CharacterInstance(CharacterCombatData characterData, float maxHP)
    {
        data = characterData;
        currentHP = maxHP;
    }
}

public class EnemyInstance : CreatureInstance
{
    public new EnemyCombatData data
    {
        get { return (EnemyCombatData) _data; }
        protected set { _data = value; }
    }

    protected DamageType type;

    public EnemyInstance(EnemyCombatData enemyData, float maxHP)
    {
        data = enemyData;
        currentHP = maxHP;
    }

    public float GetDamageEffectiveness(DamageData damage)
    {
        switch(damage.damageType)
        {
            case DamageType.Arcane:
                if(type == DamageType.Light)
                    return 2f;
                if(type == DamageType.Dark)
                    return 0.5f;
                return 1f;
            
            case DamageType.Light:
                if(type == DamageType.Dark)
                    return 2f;
                if(type == DamageType.Arcane)
                    return 0.5f;
                return 1f;
            
            case DamageType.Dark:
                if(type == DamageType.Arcane)
                    return 2f;
                if(type == DamageType.Light)
                    return 0.5f;
                return 1f;
            
            default:
                return 1f;
        }
    }

    public override bool DealDamage(DamageData damage)
    {
        damage.damageAmount = damage.damageAmount * GetDamageEffectiveness(damage);
        return base.DealDamage(damage);
    }

    public ActionData GetNextAction()
    {
        return data.Actions[Random.Range(0, data.Actions.Count)];
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

    [SerializeField] private GameObject enemyPrefab;

    public SimplePriorityQueue<CreatureInstance, float> turnOrder { get; private set; }
    private List<CharacterInstance> characterInstances;
    private List<EnemyInstance> enemyInstances;
    private List<GameObject> enemySprites;
    private float currentTurn = 0;

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
            Debug.LogWarning("No encounter data provided.");
        }
        else
        {
            foreach(EnemyCombatData enemyData in encounter.enemies)
            {
                EnemyInstance enemy = new EnemyInstance(enemyData, enemyData.MaxHP);
                enemyInstances.Add(enemy);
                turnOrder.Enqueue(enemy, enemyData.TurnLength);
            }
        }

        AddEnemiesToBattlefield();
    }

    public void AddEnemiesToBattlefield()
    {
        int numPoints = enemyInstances.Count + 1;
        for(int i = 0; i < enemyInstances.Count; ++i)
        {
            // UIManager.instance.combatUI.SpawnEnemyOfType(); // TODO: Pass in the sprites
        }
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

    public CharacterInstance GetCharacter(EntityID entityID)
    {
        return characterInstances.Find((CharacterInstance c) => c.data.CharacterID == entityID);
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

    public void QueueChargedActionForCurrentTurn(QueuedAction action, float delay)
    {
        turnOrder.First.QueueChargedAction(action);
        RequeueCurrentTurn(delay);
    }

    public void RequeueCurrentTurn(float delay)
    {
        turnOrder.Enqueue(turnOrder.Dequeue(), currentTurn + delay);
    }

    public void StartCurrentTurn()
    {
        if(turnOrder.First is CharacterInstance)
        {
            // Do the character thing
        }
        else if(turnOrder.First is EnemyInstance)
        {
            EnemyInstance enemy = (EnemyInstance)turnOrder.First;
            enemy.GetNextAction().PerformAction(enemy, GetCharacter(Random.Range(0, characterInstances.Count))).Invoke();
            RequeueCurrentTurn(enemy.data.TurnLength);
            StartCurrentTurn();
        }
        else
        {
            // Cry
        }
    }
}
