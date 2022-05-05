using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeakerID{
    MainCharacter,
    Aeris,
    Samara,
    enumSize
}

[CreateAssetMenu(menuName = "ScriptableObjects/SpeakerData")]
public class SpeakerData : ScriptableObject
{
    [SerializeField] private SpeakerID speakerID;

    [SerializeField] private Sprite portraitNeutral;
    [SerializeField] private Sprite portraitSecondary;

    [SerializeField] private int maxHealth;
    [SerializeField] private string skillPointName;

    // Unless these are going in their own object somewhere
    [SerializeField] private string action1Name;
    [SerializeField] private string action2Name;
    [SerializeField] private string action3Name;

    [SerializeField] private string special1Name;
    [SerializeField] private string special2Name;
    [SerializeField] private string special3Name;

    public SpeakerID SpeakerID()
    {
        return speakerID;
    }
    
    public Sprite PortraitNeutral()
    {
        return portraitNeutral;
    }

    public Sprite PortraitSecondary()
    {
        return portraitSecondary;
    }

    public int MaxHealth()
    {
        return maxHealth;
    }

    public string SkillPointName()
    {
        return skillPointName;
    }
}
