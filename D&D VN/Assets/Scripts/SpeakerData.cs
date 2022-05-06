using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityID{
    MainCharacter,
    Aeris,
    Samara,

    // Enemies
    LightMinion,
    DarkMinion,
    ArcanaMinion,
    DragonBoss,

    enumSize
}

[CreateAssetMenu(menuName = "VN Stuff/SpeakerData")]
public class SpeakerData : ScriptableObject
{
    [SerializeField] private EntityID entityID;

    [SerializeField] private Sprite portraitNeutral;
    [SerializeField] private Sprite portraitSecondary;

    public EntityID EntityID()
    {
        return entityID;
    }
    
    public Sprite PortraitNeutral()
    {
        return portraitNeutral;
    }

    public Sprite PortraitSecondary()
    {
        return portraitSecondary;
    }
}
