using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ChargeableActionData : ActionData
{
    public float MinChargeLengthMultiplier {get {return minChargeLengthMultiplier;}}
    public float MaxChargeLengthMultiplier {get {return maxChargeLengthMultiplier;}}

    [SerializeField] [Tooltip("The minimum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float minChargeLengthMultiplier = 0f;
    [SerializeField] [Tooltip("The maximum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float maxChargeLengthMultiplier = 0.5f;
}
