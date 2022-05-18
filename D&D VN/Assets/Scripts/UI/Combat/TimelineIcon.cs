using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineIcon : MonoBehaviour
{
    public EntityID entityID {get; private set;}
    [SerializeField] private Image iconImage;

    public float turnTriggered {get; private set;}

    public void SetTimelineIconValues( EntityID _id, Sprite _icon, float _turn )
    {
        entityID = _id;
        turnTriggered = _turn;

        iconImage.sprite = _icon;
        iconImage.preserveAspect = true;
    }

    public void GrayOutIcon()
    {
        UIManager.SetImageColorFromHex(iconImage, UIManager.MED_BROWN_COLOR);
    }

    public void SetIconNormalColor()
    {
        UIManager.SetImageColorFromHex(iconImage, "#FFFFFF");
    }
}
