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
    private abstract class CreatureInstance
    {
        protected CreatureCombatData _data;
        public float currentHP;

        public bool IsAlive()
        {
            return currentHP > 0;
        }

        public virtual bool DealDamage(DamageData damage)
        {
            currentHP -= damage.damageAmount * (_data.defense/100);
            if(currentHP < 0)
            {
                currentHP = 0;
            }

            return IsAlive();
        }
    }

    private class CharacterInstance : CreatureInstance
    {
        public CharacterCombatData data
        {
            get { return (CharacterCombatData) _data; }
            set { _data = value; }
        }

        public CharacterInstance(CharacterCombatData characterData, float maxHP)
        {
            data = characterData;
            currentHP = maxHP;
        }
    }

    private class EnemyInstance : CreatureInstance
    {
        public EnemyCombatData data
        {
            get { return (EnemyCombatData) _data; }
            set { _data = value; }
        }

        public DamageType type;

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
    }

    [System.Serializable]
    private struct SpawnBounds
    {
        public Transform leftBound;
        public Transform rightBound;
    }

    [Tooltip("The list of data for all characters to be included in this combat.")]
    public List<CharacterCombatData> characterDatas;
    
    [Tooltip("The encounter object for the enemies to be used for this combat.")]
    public EncounterData encounter;

    [Tooltip("The left and right bounds for where enemy sprites will appear on screen.")]
    [SerializeField] private SpawnBounds enemySpawnBounds;

    [SerializeField] private GameObject enemyPrefab;

    private SimplePriorityQueue<CreatureInstance, float> turnOrder;
    private List<CharacterInstance> characterInstances;
    private List<EnemyInstance> enemyInstances;
    private List<GameObject> enemySprites;
    private float currentTurn = 0;

    void Awake()
    {
        turnOrder = new SimplePriorityQueue<CreatureInstance, float>();
        characterInstances = new List<CharacterInstance>();
        enemyInstances = new List<EnemyInstance>();
        enemySprites = new List<GameObject>();
    }

    void Start()
    {
        foreach(CharacterCombatData data in characterDatas)
        {
            CharacterInstance character = new CharacterInstance(data, data.maxHP);
            characterInstances.Add(character);
            turnOrder.Enqueue(character, data.turnLength);
        }

        if(encounter == null)
        {
            Debug.LogWarning("No encounter data provided.");
        }
        else
        {
            foreach(EnemyCombatData enemyData in encounter.enemies)
            {
                EnemyInstance enemy = new EnemyInstance(enemyData, enemyData.maxHP);
                enemyInstances.Add(enemy);
                turnOrder.Enqueue(enemy, enemyData.turnLength);
            }
        }

        AddEnemiesToBattlefield();
    }

    public void AddEnemiesToBattlefield()
    {
        int numPoints = enemyInstances.Count + 1;
        for(int i = 0; i < enemyInstances.Count; ++i)
        {
            enemySprites.Add(Instantiate(enemyPrefab, Vector3.Lerp(enemySpawnBounds.leftBound.position, enemySpawnBounds.rightBound.position, (i+1f) / numPoints), Quaternion.identity));
        }
    }

    public void DealDamageToEnemy(int index, DamageData damage)
    {
        if(index >= enemyInstances.Count || index < 0)
        {
            Debug.LogWarning("Attempted to deal damage to an enemy out of bounds of enemy array");
        }

        enemyInstances[index].DealDamage(damage);
    }

    public void DealDamageToCharacter(int index, DamageData damage)
    {
        if(index >= characterInstances.Count || index < 0)
        {
            Debug.LogWarning("Attempted to deal damage to a character out of bounds of character array");
        }

        characterInstances[index].DealDamage(damage);
    }

    public void DealDamageToRandomCharacter(DamageData damage)
    {
        DealDamageToCharacter(Random.Range(0, characterInstances.Count), damage);
    }
}
