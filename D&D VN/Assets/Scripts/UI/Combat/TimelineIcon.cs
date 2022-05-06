using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineIcon : MonoBehaviour
{
    public EntityID entityID {get; private set;}
    [SerializeField] private Image iconImage;

    public int turnTriggered {get; private set;}

    public void SetTimelineIconValues( EntityID _id, Sprite _icon, int _turn )
    {
        entityID = _id;
        turnTriggered = _turn;

        iconImage.sprite = _icon;
        iconImage.preserveAspect = true;
    }

    public void GrayOutIcon()
    {
        // TODO: Gray it out
        // iconImage.color
    }

    public void SetIconNormalColor()
    {
        iconImage.color = new Color(255,255,255,255);
    }
}
