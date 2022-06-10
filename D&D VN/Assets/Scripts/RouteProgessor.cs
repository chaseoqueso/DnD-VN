using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteProgessor : MonoBehaviour
{
    public void AerisProgression()
    {
        GameManager.instance.IncrementAerisRoute();
    }

    public void AerisGoodChoice()
    {
        GameManager.instance.GrowCloserToAeris();
    }

    public void CheckAerisGoodEnding()
    {
        GameManager.instance.SetAerisGoodEnding();
    }

    public void SamaraProgression()
    {
        GameManager.instance.IncrementSamaraRoute();
    }

    public void SamaraGoodChoice()
    {
        // GameManager.instance.GrowCloserToSamara();
    }

    public void CheckSamaraGoodEnding()
    {
        GameManager.instance.SetSamaraGoodEnding();
    }
}
