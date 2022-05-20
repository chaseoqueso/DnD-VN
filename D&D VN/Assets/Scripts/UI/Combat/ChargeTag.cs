using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeTag : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetValues(Sprite img)
    {
        icon.sprite = img;
    }
}
