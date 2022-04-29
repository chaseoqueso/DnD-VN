using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<CharacterCombatData> characterDatas;

    private Dictionary<float, CreatureInstance> turnOrder;
    private List<CharacterInstance> characterInstances;
    private float currentTurn = 0;

    void Awake()
    {
        turnOrder = new Dictionary<float, CreatureInstance>();
        characterInstances = new List<CharacterInstance>();
    }

    void Start()
    {
        foreach(CharacterCombatData data in characterDatas)
        {
            characterInstances.Add(new CharacterInstance(data, data.maxHP));
        }
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
