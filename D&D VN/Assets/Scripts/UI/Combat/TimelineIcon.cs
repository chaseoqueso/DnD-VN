using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineIcon : MonoBehaviour
{
    public EntityID entityID {get; private set;}
    [SerializeField] private Image iconImage;

    public void SetTimelineIconValues( EntityID _id, Sprite _icon )
    {
        entityID = _id;

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
